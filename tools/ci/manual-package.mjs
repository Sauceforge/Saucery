import {entryForProject,policy,output} from './lib.mjs';
const project=process.env.PROJECT;
if(!policy.manualPackages.includes(project))throw new Error('Package is not in the allowlist');
const f=entryForProject(project);
output('packageId',f.packageId);
output('tag',f.tag);
