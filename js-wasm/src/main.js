
fetch('../out/main.wasm')
	.then(response =>
		response.arrayBuffer()
	)
	.then(bytes => 
		WebAssembly.instantiate(bytes)
	)
	.then(results => {
		const instance = results.instance;
		
		document.getElementById("main").textContent = instance.exports.main();
		document.getElementById("result").textContent = instance.exports.add(4, 6);

		const buffer = new Uint8Array(instance.exports.memory.buffer);
		const pointer = instance.exports.greet();
		
		let message = "";
		for (let i = pointer; String.fromCharCode(buffer[i]) !== '\0'; i++) {
			message += String.fromCharCode(buffer[i]);
		}
		
		document.getElementById("greeting").textContent = message;
		
	}).catch(console.error);
