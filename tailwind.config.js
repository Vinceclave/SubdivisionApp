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
            spacing: {
                '12': '3rem',
                '24': '6rem',
            }
        },
    },
    variants: {
        extend: {},
    },
    plugins: [
        require('tailwindcss-textshadow')
    ],
}
