# Runinng WebAssembly module in HTML using JavaScript
This example is based on default Empty C project from [webassembly.studio](https://webassembly.studio/). It contains few additional simple methods (like greet) to show how handle strings in WebAssembly.

# How to run
Simply use some local http servers to serve like [Live Server](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer) extension for Visual Studio Code.

# Note
Repository includes compiled wasm file and used by the code. If you do any changes in C code you need to install building tools for C and Web Assembly like [Emscripten](https://emscripten.org/). Or you can simply make changes in [webassembly.studio](https://webassembly.studio/), build it and download it locally.