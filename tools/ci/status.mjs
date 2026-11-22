import {maybeStatus} from './lib.mjs';
const [key,result]=process.argv.slice(2);
if(!/^(NUnit|XUnit|XUnit3|TUnit):(virtual|real)$/.test(key))throw new Error('Invalid suite ID');
maybeStatus(key,result==='success');
