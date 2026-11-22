import test from 'node:test';
import assert from 'node:assert/strict';
import {commandFor} from '../Command.mjs';

test('Windows npm install never passes double-quoted subcommand', () => {
  const cmd=commandFor('npm',['install','--package-lock-only','--ignore-scripts','--no-audit','--no-fund'],'win32','cmd.exe');
  assert.deepEqual(cmd.args, ['/d','/s','/c','npm install --package-lock-only --ignore-scripts --no-audit --no-fund']);
  assert.equal(cmd.program,'cmd.exe');
  assert.equal(cmd.windowsVerbatimArguments,true);
  assert.ok(!cmd.args[3].includes('"'));
});
test('Windows npm ci uses the same safe command construction', () => {
  const cmd=commandFor('npm',['ci','--no-audit','--no-fund'],'win32');
  assert.equal(cmd.args[3],'npm ci --no-audit --no-fund');
  assert.equal(cmd.windowsVerbatimArguments,true);
});
test('Installer uses native .NET executable on Windows', () => {
  const cmd=commandFor('dotnet',['restore','Saucery.slnx'],'win32');
  assert.deepEqual(cmd,{program:'dotnet.exe',args:['restore','Saucery.slnx'],windowsVerbatimArguments:false});
});
test('POSIX execution remains direct without an intermediate shell', () => {
  const cmd=commandFor('npm',['ci'],'linux');
  assert.deepEqual(cmd,{program:'npm',args:['ci'],windowsVerbatimArguments:false});
});
test('Unsafe future npm arguments fail closed', () => {
  assert.throws(()=>commandFor('npm',['install','some && echo bad'],'win32'),/Unsafe npm argument/);
});
