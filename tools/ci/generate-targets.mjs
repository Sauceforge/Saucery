import fs from 'node:fs';
import {policy} from './lib.mjs';
const targets=new Map();
function add(project,key,command) {
 if(!targets.has(project))targets.set(project,{});
 targets.get(project)[key]={"executor":"nx:run-commands","options":{"command":command,"cwd":"{workspaceRoot}"},"dependsOn":["build"],"cache":false};
}
add('Saucery.Core.Tests.XUnitv3','ci-unit','node tools/ci/offline.mjs unit');
add('Template.Tests','ci-template','node tools/ci/offline.mjs template');
for(const f of Object.values(policy.frameworks))add('Template.Tests',`ci-template-${f.templateCategory}`,`node tools/ci/offline.mjs template ${f.templateCategory}`);
add(policy.nuget.tests,'ci-nuget-unit','node tools/ci/offline.mjs nuget');
for(const [name,f] of Object.entries(policy.frameworks)) for(const [i,p] of f.external.entries()) {
 add(p,'ci-external',`node tools/ci/suite.mjs external ${name} ${i?'real':'virtual'}`);
}
for(const kind of ['reporter','remediator'])add(`Saucery.CoverageGap${kind==='reporter'?'Reporter':'Remediator'}.Tests`,'ci-tool-test',`node tools/ci/tool.mjs ${kind}`);
for(const [project,definition] of targets){
 const folder=project;
 if(!fs.existsSync(`${folder}/${project}.csproj`))throw new Error(`Cannot create Nx target: missing ${project}/${project}.csproj`);
 const dest=`${folder}/project.json`;
 if(fs.existsSync(dest))throw new Error(`Refusing to overwrite existing ${dest}`);
 fs.writeFileSync(dest,JSON.stringify({name:project,targets:definition},null,2)+'\n');
 console.log(`Configured ${project} (${Object.keys(definition).join(', ')})`);
}
