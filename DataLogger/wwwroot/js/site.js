const text =
    "E se você pudesse produzir o queijo perfeito?";

const typingText = document.getElementById("typing-text");

let index = 0;

function typeWriter() {

    if (index < text.length) {

        typingText.innerHTML += text.charAt(index);

        index++;

        setTimeout(typeWriter, 60);
    }
}

window.onload = typeWriter;