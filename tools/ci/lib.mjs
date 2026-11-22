import fs from 'node:fs';
import path from 'node:path';
import { spawnSync } from 'node:child_process';
export const policy=JSON.parse(fs.readFileSync(new URL('../../ci/policy.json',import.meta.url),'utf8'));
export function exec(command,args=[],opts={}) {
 const r=spawnSync(command,args,{encoding:'utf8',shell:false,stdio:opts.capture?'pipe':'inherit',env:process.env,...opts});
 if(r.error)throw r.error;
 if(r.status!==0) throw new Error(`${command} ${args.join(' ')} exited ${r.status}${r.stderr ? ': '+r.stderr:''}`);
 return r.stdout?.trim()??'';
}
export function output(k,v){if(process.env.GITHUB_OUTPUT)fs.appendFileSync(process.env.GITHUB_OUTPUT,`${k}=${typeof v==='string'?v:JSON.stringify(v)}\n`); else console.log(k,v)}
export function entryForProject(project){if(project===policy.core.project)return {...policy.core}; if(project===policy.nuget.project)return {...policy.nuget};return Object.values(policy.frameworks).find(f=>f.project===project)}
export function suites(kind='internal'){
 return Object.entries(policy.frameworks).flatMap(([framework, f])=> f[kind].map((project,i)=>({id:`${framework}:${i?'real':'virtual'}`, framework,device:i?'real':'virtual', project, runner:f.runner, kind, coverage:`cobertura.${framework.toLowerCase()}${i?'.realdevice':''}.integration.xml`})));
}
export function releaseEligibility(requested, successes, unitPassed=true){
 return requested.filter(p=> {
  if(!unitPassed) return false;
  const required=p===policy.core.project ? policy.core.requires : Object.entries(policy.frameworks).find(([,f])=>f.project===p)?.[1]?.internal.map((_,i)=>`${Object.entries(policy.frameworks).find(([,f])=>f.project===p)[0]}:${i?'real':'virtual'}`) || [];
  return required.length>0 && required.every(key=>successes.has(key));
 });
}
export function maybeStatus(suiteId, ok){
 fs.mkdirSync('ci-results/status',{recursive:true});
 fs.writeFileSync(`ci-results/status/${suiteId.replace(':','-')}.json`,JSON.stringify({suite:suiteId,ok:!!ok})+'\n');
}
