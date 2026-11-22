import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import { policy,suites,releaseEligibility,entryForProject } from '../tools/ci/lib.mjs';
const internal=suites('internal'), external=suites('external');
test('four distinct framework policies with two internal and external device suites',()=>{
 assert.equal(Object.keys(policy.frameworks).length,4);
 assert.equal(internal.length,8);
 assert.equal(external.length,8);
 assert.equal(new Set(internal.map(x=>x.project)).size,8);
 assert.equal(new Set(external.map(x=>x.project)).size,8);
});
test('xUnit v3 package identity is not inferred from its project name',()=>{
 assert.equal(policy.frameworks.XUnit3.packageId,'Saucery.XUnit.v3');
 assert.equal(entryForProject('Saucery.XUnit3').packageId,'Saucery.XUnit.v3');
});
test('Core is withheld unless all eight suites succeed',()=>{
 const full=new Set(internal.map(x=>x.id));
 assert.deepEqual(releaseEligibility([policy.core.project],full,true),[policy.core.project]);
 for(const missing of internal.map(x=>x.id)){
  const partial=new Set(full);partial.delete(missing);
  assert.deepEqual(releaseEligibility([policy.core.project],partial,true),[],`Core wrongly passed without ${missing}`);
 }
});
test('Each adapter requires only its own virtual and real tests',()=>{
 for(const [framework,f] of Object.entries(policy.frameworks)){
  const pair=new Set([`${framework}:virtual`,`${framework}:real`]);
  assert.deepEqual(releaseEligibility([f.project],pair,true),[f.project]);
  assert.deepEqual(releaseEligibility([f.project],new Set([`${framework}:virtual`]),true),[]);
  assert.deepEqual(releaseEligibility([f.project],new Set([`${framework}:real`]),true),[]);
 }
});
test('Unit or build failure blocks every automatic release',()=>{
 const all=new Set(internal.map(x=>x.id));
 const candidates=[policy.core.project,...Object.values(policy.frameworks).map(f=>f.project)];
 assert.deepEqual(releaseEligibility(candidates,all,false),[]);
 assert.equal(releaseEligibility(candidates,all,true).length,5);
});
test('Published-package suites are distinct from local project integrations',()=>{
 for(const item of external)assert.ok(item.project.startsWith('ExternalMerlin.'));
 for(const item of internal)assert.ok(item.project.startsWith('Merlin.'));
});
test('NuGet sync order retains dependency and version sequencing',()=>{
 assert.deepEqual(policy.syncOrder.map(x=>x[1]),['Saucery.Core','Saucery.TUnit','Saucery.XUnit3','Saucery.XUnit','Saucery','Saucery.Playwright.NUnit','Saucery.NuGet']);
});
test('Nx Cloud must be disabled',()=>{
 const config=JSON.parse(fs.readFileSync(new URL('../nx.json',import.meta.url)));
 assert.equal(config.neverConnectToCloud,true);
});
