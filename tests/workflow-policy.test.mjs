import test from 'node:test';
import assert from 'node:assert/strict';
import { spawnSync } from 'node:child_process';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
const root=path.resolve(path.dirname(fileURLToPath(import.meta.url)),'..');
function plan(lane,event,clock='',selected='all'){
 const r=spawnSync(process.execPath,['tools/ci/plan.mjs',lane],{cwd:root,encoding:'utf8',env:{...process.env,GITHUB_EVENT_NAME:event,GITHUB_SCHEDULE:clock,CI_FAMILY:selected}});
 assert.equal(r.status,0,r.stderr);
 const line=r.stdout.trim().split('\n').pop();
 return JSON.parse(line);
}
test('Original framework schedule selects four adapter packages but not Core',()=>{
 const p=plan('internal','schedule','0 1,7,13,19 * * *');
 assert.equal(p.suites.length,8);
 assert.equal(p.requested.length,4);
 assert.ok(!p.requested.includes('Saucery.Core'));
});
test('Original Core schedule requires all eight suites, and only Core is requested',()=>{
 const p=plan('internal','schedule','0 2,8,14,20 * * *');
 assert.equal(p.suites.length,8);
 assert.deepEqual(p.requested,['Saucery.Core']);
});
test('Manual TUnit request runs exactly its real/virtual pair',()=>{
 const p=plan('internal','workflow_dispatch','','TUnit');
 assert.deepEqual(p.suites,['TUnit:virtual','TUnit:real']);
 assert.deepEqual(p.requested,['Saucery.TUnit']);
});
test('External checks never request local source NuGet publication',()=>{
 const p=plan('external','schedule','0 2,8,14,20 * * *');
 assert.equal(p.suites.length,8);
 assert.deepEqual(p.requested,[]);
});
function gate(requested,successes,unit='success',build='success'){
 const tmp=fs.mkdtempSync(path.join(os.tmpdir(),'saucery-nx-gate-'));
 try{
  const dir=path.join(tmp,'ci-results/downloaded-status');fs.mkdirSync(dir,{recursive:true});
  successes.forEach((suite,i)=>fs.writeFileSync(path.join(dir,`suite-${i}.json`),JSON.stringify({suite,ok:true})));
  const output=path.join(tmp,'outputs.txt');fs.writeFileSync(output,'');
  const r=spawnSync(process.execPath,[path.join(root,'tools/ci/gate.mjs')],{cwd:tmp,encoding:'utf8',env:{...process.env,REQUESTED_PACKAGES:requested.join(','),UNIT_RESULT:unit,BUILD_RESULT:build,GITHUB_OUTPUT:output}});
  assert.equal(r.status,0,r.stderr);
  const pairs=fs.readFileSync(output,'utf8').trim().split('\n').map(x=>{const i=x.indexOf('=');return [x.slice(0,i),x.slice(i+1)]});
  return Object.fromEntries(pairs);
 }finally{fs.rmSync(tmp,{recursive:true,force:true})}
}
test('Release fails closed for missing suite, even if other suites pass',()=>{
 const result=gate(['Saucery.Core'],['NUnit:virtual']);
 assert.equal(result.has_release,'false');
 assert.equal(result.has_report,'true');
});
test('Core releases only after all 8 successful statuses and successful build/unit',()=>{
 const suites=['NUnit','XUnit','XUnit3','TUnit'].flatMap(x=>[`${x}:virtual`,`${x}:real`]);
 assert.equal(gate(['Saucery.Core'],suites).has_release,'true');
 assert.equal(gate(['Saucery.Core'],suites,'failure').has_release,'false');
});
