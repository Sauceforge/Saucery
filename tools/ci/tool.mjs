import { exec } from './lib.mjs';
const [type]=process.argv.slice(2);
const project= type==='reporter'?'Saucery.CoverageGapReporter':type==='remediator'?'Saucery.CoverageGapRemediator':null;
if(!project)throw new Error('Expected reporter or remediator');
const args=['run','--project',`${project}.Tests/${project}.Tests.csproj`,'--configuration','Release','--','--report-trx','--results-directory',`${process.cwd()}/TestResults/coverage-gap-tools`];
exec('dotnet',args);
