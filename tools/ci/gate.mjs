import fs from 'node:fs';
import {policy,releaseEligibility,output} from './lib.mjs';
const requested=(process.env.REQUESTED_PACKAGES||'').split(',').filter(Boolean);
const successes=new Set();
const root='ci-results/downloaded-status';
if(fs.existsSync(root))for(const file of fs.readdirSync(root,{recursive:true})) {
 if(!file.endsWith('.json'))continue;
 const item=JSON.parse(fs.readFileSync(`${root}/${file}`,'utf8'));
 if(item.ok===true)successes.add(item.suite);
}
const approved=releaseEligibility(requested,successes,process.env.UNIT_RESULT==='success' && process.env.BUILD_RESULT==='success');
const available=approved.map(p=>{
 const f=p===policy.core.project?policy.core:Object.values(policy.frameworks).find(x=>x.project===p);
 if(!f)throw new Error(`Unknown approved package ${p}`);
 return {project:p,packageId:f.packageId,tag:f.tag};
});
const reports=requested.map(p=>{
 const f=p===policy.core.project?policy.core:Object.values(policy.frameworks).find(x=>x.project===p);
 if(!f)throw new Error(`Unknown requested report package ${p}`);
 return {project:p,packageId:f.packageId,tag:f.tag};
});
output('release_matrix',{include:available.length?available:[{project:'none',packageId:'none',tag:''}]});
output('report_matrix',{include:reports.length?reports:[{project:'none',packageId:'none',tag:''}]});
output('has_release',available.length?'true':'false');
output('has_report',reports.length && successes.size ? 'true':'false');
output('approved',approved.join(','));
output('success_count',successes.size.toString());
console.log(JSON.stringify({requested,successes:[...successes],approved}));
