import org.wasmer.Instance;
import org.wasmer.Memory;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.file.Files;
import java.nio.file.Paths;

public class Main {
    public static void main(String[] args) throws IOException {

        System.out.println("Calling methods exported by WebAssembly...");

        byte[] bytes = Files.readAllBytes(Paths.get("src/main/resources/main.wasm"));
        Instance instance = new Instance(bytes);

        // main method
        Object[] mainResults = instance.exports.getFunction("main").apply();
        System.out.println("Result of main: " + mainResults[0]);

        // add method
        Object[] addResults = instance.exports.getFunction("add").apply(5, 7);
        System.out.println("Result of add: " + addResults[0]);

        // greet method
        Object[] greetResults = instance.exports.getFunction("greet").apply();
        Memory memory = instance.exports.getMemory("memory");
        ByteBuffer buffer = memory.buffer();
        Integer pointer = (Integer)greetResults[0];
        buffer.position(pointer);

        int pos = pointer;
        StringBuilder sb = new StringBuilder();
        while (buffer.get(pos) !='\0') {
            byte b = buffer.get(pos);
            sb.append((char)b);
            pos++;
        }
        System.out.println("Result of greet: " + sb.toString());

        instance.close();
    }
}
