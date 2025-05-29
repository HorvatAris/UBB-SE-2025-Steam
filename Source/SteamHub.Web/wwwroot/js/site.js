﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function loadGameReviews({gameId, sortOption, recommendationFilter}){
    const queryString = new URLSearchParams({gameId, sortOption, recommendationFilter}).toString();
    const response = await fetch(`http://localhost:5203/Reviews?${queryString}`);
    if (response.ok){
        const html= await response.text()
        const tempEl = $('<div></div>');
        tempEl.html(html);
        $('.reviews-container select', tempEl).each((index, elem) =>{
            elem.removeAttribute('onchange')
        })
        

        $('.reviews-container select', tempEl).change((e) => {
            e.preventDefault();
            
            const params = Object.assign({}, ...$('.reviews-container select').toArray().map(elem =>{
                const kv = {}
                kv[elem.name] = elem.value;
                return kv;
            }))
            loadGameReviews({gameId, ...params});
        })
        $('.reviews-container>div:first-child', tempEl).remove();
        $('.reviews-container .game-title', tempEl).remove();
        
        $('.reviews-container').replaceWith($('.reviews-container', tempEl));
    }
}