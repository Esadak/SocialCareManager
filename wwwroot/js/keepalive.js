// Pinga webbservern var 4:e minut för att förhindra att Render lägger den i vila
setInterval(async () => {
    try {
        // Vi pingar roten '/' på samma domän. Inga CORS-problem, inga 404.
        const response = await fetch('/');
        if (response.ok) {
            console.log('Keep-alive ping successful');
        }
    } catch (error) {
        // Ignorera fel tyst så det inte skräpar ner konsolen
    }
}, 4 * 60 * 1000); // 4 minuter (240 000 millisekunder)