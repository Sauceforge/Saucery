import { policy,suites,exec,output } from './lib.mjs';
import fs from 'node:fs';
const lane=process.argv[2];
if(!['internal','external'].includes(lane))throw new Error('Expected internal or external');
const event=process.env.GITHUB_EVENT_NAME||'workflow_dispatch';
const schedule=process.env.GITHUB_SCHEDULE||'';
const selected=process.env.CI_FAMILY||'all';
const frameworkEntries=Object.entries(policy.frameworks);
const force=event==='workflow_dispatch' || event==='schedule';
let affected=new Set();
if(!force){
 const base=process.env.NX_BASE,head=process.env.NX_HEAD;
 if(!base||!head)throw new Error('Affected selection requires NX_BASE and NX_HEAD (fetch full Git history).');
 const raw=exec(process.execPath,['tools/ci/query-affected.mjs',base,head],{capture:true});
 affected=new Set(JSON.parse(raw));
}
let desiredFrameworks=frameworkEntries.filter(([name,f])=>{
 if(selected!=='all'&&name!==selected)return false;
 if(force){
  if(lane==='external')return true;
  // Two original schedule families: framework = 01/07/13/19 UTC; Core = 02/08/14/20 UTC.
  return true;
 }
 return f[ lane ].some(project=>affected.has(project)) || affected.has(f.project) ||
        (lane==='internal' && (affected.has('Saucery.Core') || affected.has('Template.Tests') || affected.has('Saucery.Core.Tests.XUnitv3')));
}).map(([name])=>name);
let coreRequested=lane==='internal' && (force ? (selected==='all' && (event!=='schedule'||schedule==='0 2,8,14,20 * * *')) : affected.has('Saucery.Core')||affected.has('Saucery.Core.Tests.XUnitv3')||affected.has('Template.Tests'));
if(event==='schedule'&&lane==='internal'&&schedule==='0 2,8,14,20 * * *')desiredFrameworks=frameworkEntries.map(([k])=>k);
if(event==='schedule'&&lane==='internal'&&schedule==='0 1,7,13,19 * * *')coreRequested=false;
if(coreRequested)desiredFrameworks=frameworkEntries.map(([k])=>k);
const selectedSuites=suites(lane).filter(s=>desiredFrameworks.includes(s.framework));
const requested=lane==='internal' ? [...new Set([...(coreRequested?[policy.core.project]:[]),...desiredFrameworks.map(k=>policy.frameworks[k].project).filter(()=> !(event==='schedule'&&schedule==='0 2,8,14,20 * * *'))])] : [];
const dummy={id:'none',project:'none',framework:'none',device:'none',runner:'none',kind:lane,coverage:'none'};
output('matrix',{include:selectedSuites.length?selectedSuites:[dummy]});
output('has_suites',selectedSuites.length?'true':'false');
output('projects',selectedSuites.map(s=>s.project).join(','));
output('requested',requested.join(','));
output('core_requested',coreRequested?'true':'false');
console.log(JSON.stringify({lane,event,schedule,suites:selectedSuites.map(s=>s.id),requested}));
