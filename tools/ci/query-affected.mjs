import { spawnSync } from 'node:child_process';
const [base,head]=process.argv.slice(2);
const r=spawnSync(process.execPath,['tools/ci/nx.mjs','show','projects','--affected','--base',base,'--head',head,'--json'],{encoding:'utf8',shell:false});
if(r.status!==0){console.error(r.stderr||r.stdout);process.exit(r.status||1)}
const match=r.stdout.match(/\[[\s\S]*\]/);
if(!match)throw new Error('Nx affected result was not a JSON array');
console.log(JSON.stringify(JSON.parse(match[0])));
