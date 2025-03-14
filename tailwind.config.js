module.exports = {
    purge: {
        enabled: true,
        content: [
            './Pages/**/*.cshtml',
            './Views/**/*.cshtml',
            './wwwroot/**/*.html',
            './wwwroot/**/*.js'
        ]
    },
    darkMode: false, // or 'media' or 'class'
    theme: {
        extend: {
            textShadow: {
                'custom': '8px 8px 4px rgba(0, 0, 0, 0.25)',
            },     
            colors: {
                'beverly-light-green': '#D9DED5',
                'beverly-green': '#445E4F',
                'beverly-cream': '#F9FBF3',
                'beverly-dark-green': '#374836',
                'beverly-off-white': '#FBFCF4',
              },
              fontFamily: {
                sans: ['Helvetica', 'Arial', 'sans-serif'],
              },
        },
    },
    variants: {
        extend: {},
    },
    plugins: [
        require('tailwindcss-textshadow')
    ],
}

