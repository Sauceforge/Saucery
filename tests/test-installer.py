import json, os, pathlib, subprocess, tempfile, shutil
ROOT=pathlib.Path(__file__).resolve().parents[1]
OLD=['core-pipeline','coverage-gap-remediation','coverage-gap-report','delete-sauce-errored-jobs','json-yaml-validate','nuget-total-downloads-badge','nunit-pipeline-external','nunit-pipeline','publish-core','publish-nunit','publish-saucery-nuget','publish-tunit','publish-xunit','publish-xunit3','saucery-nuget-pipeline','saucery-nuget-sync-and-commit','tunit-pipeline-diagnostic','tunit-pipeline-external','tunit-pipeline','unit-test-only','update-years-since','xunit-pipeline-external','xunit-pipeline','xunit3-pipeline-external','xunit3-pipeline']
policy=json.loads((ROOT/'ci/policy.json').read_text())
projects={'Saucery.Core','Saucery.Core.Tests.XUnitv3','Template.Tests','Saucery.NuGet.Tests','Saucery.Badges','Saucery.CoverageGapReporter','Saucery.CoverageGapRemediator','Saucery.CoverageGapReporter.Tests','Saucery.CoverageGapRemediator.Tests'}
for f in policy['frameworks'].values():projects.update([f['project'],*f['internal'],*f['external']])

def setup(tmp):
 repo=tmp/'Saucery';repo.mkdir()
 (repo/'Saucery.slnx').write_text('<Solution>\n  <Folder Name="/Solution Items/">\n' + ''.join(f'    <File Path=".github/workflows/{n}.yml" />\n' for n in OLD) + '  </Folder>\n</Solution>\n')
 (repo/'Directory.Packages.props').write_text('<Project/>')
 for p in projects:
  d=repo/p;d.mkdir();(d/(p+'.csproj')).write_text('<Project/>')
 (repo/'.github/workflows').mkdir(parents=True)
 for name in OLD: (repo/'.github/workflows'/(name+'.yml')).write_text(f'name: {name}\non:\n  workflow_dispatch:\njobs: {{}}\n')
 mock=tmp/'bin';mock.mkdir()
 dotnet=mock/'dotnet';dotnet.write_text('#!/bin/sh\necho 11.0.100\n');dotnet.chmod(0o755)
 npm=mock/'npm';npm.write_text('''#!/bin/sh
if [ "$1" = "install" ]; then printf '{}\\n' > package-lock.json; fi
if [ "$1" = "ci" ]; then
  mkdir -p node_modules/nx/dist/bin
  printf '{"bin":{"nx":"dist/bin/nx.js"}}' > node_modules/nx/package.json
  printf '%s\\n' "if(process.env.FAIL_NX === '1' && process.argv.includes('run'))process.exit(1);console.log('FAKE NX OK')" > node_modules/nx/dist/bin/nx.js
fi
exit 0
''');npm.chmod(0o755)
 return repo,mock

def run(fail):
 with tempfile.TemporaryDirectory(prefix='nx-expert-') as temp:
  tmp=pathlib.Path(temp)
  repo,mock=setup(tmp)
  env={**os.environ,'PATH':str(mock)+':'+os.environ['PATH'],'FAIL_NX':str(int(fail))}
  proc=subprocess.run(['node',str(ROOT/'Install.mjs'),str(repo)],env=env,capture_output=True,text=True,timeout=60)
  print('failed simulation' if fail else 'successful simulation',proc.returncode)
  if fail:
   assert proc.returncode!=0,proc.stdout+proc.stderr
   assert all((repo/'.github/workflows'/(n+'.yml')).exists() for n in OLD)
   assert not (repo/'nx.json').exists()
   assert not (repo/'package.json').exists()
   assert not (repo/'Saucery.Core.Tests.XUnitv3/project.json').exists()
  else:
   assert proc.returncode==0,proc.stdout+proc.stderr
   assert not any((repo/'.github/workflows'/(n+'.yml')).exists() for n in OLD)
   assert len(list((repo/'.github/workflows').glob('*.yml')))==9
   assert (repo/'Template.Tests/project.json').exists()
   assert (repo/'NX-MIGRATION-INTENT-AUDIT.json').exists()
   assert (repo/'package-lock.json').exists()
  return proc.stdout+proc.stderr

if __name__=='__main__':
 print(run(False)[-1500:]);print(run(True)[-1200:]);print('INSTALLER INTEGRATION SIMULATIONS PASS')
