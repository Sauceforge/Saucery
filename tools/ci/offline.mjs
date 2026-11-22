import {exec} from './lib.mjs';
const [kind,category]=process.argv.slice(2);
if(kind==='unit'){
 exec('dotnet',['run','-c','Release','--project','Saucery.Core.Tests.XUnitv3/Saucery.Core.Tests.XUnitv3.csproj','--','--coverage','--coverage-output-format','cobertura','--coverage-output','cobertura.unit.xml']);
}else if(kind==='template'){
 const args=['run','-c','Release','--project','Template.Tests/Template.Tests.csproj'];
 if(category){if(!['NUnit','XUnit','XUnit3','TUnit'].includes(category))throw new Error('Unknown template category');args.push('--','--treenode-filter',`/*/*/*/*[Category=${category}]`)}
 exec('dotnet',args);
}else if(kind==='nuget'){
 exec('dotnet',['run','-c','Release','--project','Saucery.NuGet.Tests/Saucery.NuGet.Tests.csproj','--','--coverage','--coverage-output-format','cobertura','--coverage-output','cobertura.unit.xml']);
}else throw new Error('Expected unit, template or nuget');
