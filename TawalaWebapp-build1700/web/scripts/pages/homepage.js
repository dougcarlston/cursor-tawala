function startSlideshow(){
	var slideshow2 = new YAHOO.myowndb.slideshow("slideshow", {
		effect: YAHOO.myowndb.slideshow.effects.slideRight,
		interval: 5000
	});
	slideshow2.loop();
}

$E.on(window, "load", startSlideshow);
