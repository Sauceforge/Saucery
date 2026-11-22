import {exec,policy} from './lib.mjs';
const [kind,framework,device]=process.argv.slice(2);
if(kind!=='external')throw new Error('Internal suites use GitHub coverage actions to preserve their coverage semantics');
const f=policy.frameworks[framework];
if(!f||!['virtual','real'].includes(device))throw new Error('Unknown framework or device type');
const project=f.external[device==='real'?1:0];
const csproj=`${project}/${project}.csproj`;
const args=f.runner==='xunit3'||f.runner==='tunit' ? ['run','-c','Release','--project',csproj] : ['test','-c','Release',csproj];
exec('dotnet',args);
