import { createRequire } from 'node:module';
import { spawnSync } from 'node:child_process';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
const require = createRequire(import.meta.url);
const root = join(dirname(fileURLToPath(import.meta.url)), '..', '..');
const pkg = require(join(root, 'node_modules/nx/package.json'));
const executable = typeof pkg.bin === 'string' ? pkg.bin : pkg.bin.nx;
if (!executable) throw new Error('Installed Nx package has no CLI entry');
const result = spawnSync(process.execPath, [join(root,'node_modules/nx',executable), ...process.argv.slice(2)], {
  cwd:root, env: {...process.env, NX_NO_CLOUD:'true',NX_DAEMON:'false',NX_SKIP_NX_CACHE: process.env.NX_SKIP_NX_CACHE || 'false'},
  stdio:'inherit',shell:false
});
if(result.error) throw result.error;
process.exit(result.status ?? 1);
