setInterval(async () => {
    try {
        await fetch('/'); // Pinga samma domän
        console.log('Keep-alive ping successful');
    } catch (error) {
        console.log('Keep-alive ping failed:', error);
    }
}, 4 * 60 * 1000);