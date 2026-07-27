// photoView setup 
YAHOO.photoViewer.config = {
	viewers: {
		"dashboardScreenshots" : {
			properties: {
				id: "dashboardScreenshots",
				grow: 0.6,
				fade: 0.6,
				modal: true,
				dragable: false,
				fixedcenter: false,
				loadFrom: "html",
				position: "absolute",
				xy:[200,30],
				easing: YAHOO.util.Easing.easeBothStrong,
				buttonText: { next: "NEXT >", prev: "< PREVIOUS", close: "CLOSE" }
			}
		}
	}
};			

// Setup the tabs
var myTabs = new YAHOO.widget.TabView("sdinfo");

// Setup the testimonials
Tawala.testimonitalsInit = function() {
	var tBlocks, testimonial, i;
	
	tBlocks = YAHOO.util.Dom.getElementsByClassName("tawalaTestimonials");
	for(i = 0; i < tBlocks.length; i++) {
		testimonial = new Tawala.testimonials(tBlocks[i]);

		if(YAHOO.util.Dom.hasClass(tBlocks[i], "showOne")) {
			testimonial.showOne();
		}else if (YAHOO.util.Dom.hasClass(tBlocks[i], "showAll")) {
			testimonial.showAll();
		}else if (YAHOO.util.Dom.hasClass(tBlocks[i], "rotateOne")){
			testimonial.rotateOne();
		}
	}
};

Tawala.testimonials = function(target, attr) {
	this.target = target;

	if(attr && attr.rotationInterval && typeof(attr.rotationInterval === "number")){
		this.rotationInterval = attr.rotationInterval
	}else{
		this.rotationInterval = 15000;	// 15 second rotation interval by default		
	}

	if(attr && attr.testimonialList && typeof(attr.testimonialList === "object") && attr.testimonialList.length > 0){
		this.testimonialsList = attr.testimonialList;
	}else{
		this.testimonialsList = Tawala.testimonials.list;
	}
};

Tawala.testimonials.prototype = {
	lastQuoteId: "",
	
	showAll: function(){
		var tQuote = "", i, text, name, title, org;
		
		for(i = 0; i < this.testimonialsList.length; i++) {
			text = this.testimonialsList[i].text || "";
			name = this.testimonialsList[i].name || "";
			title = this.testimonialsList[i].title || "";
			org = this.testimonialsList[i].organization || "";

			if (text != "" || name != "") {
				tQuote += "<blockquote>\n";
				tQuote += "<p>\n";
				tQuote += this.testimonialsList[i].text;
				tQuote += "</p>\n<cite>-- ";
				tQuote += name;
				if (title != "") {
					tQuote += ", " + title;
				}
				if (org != "") {
					tQuote += "<br />" + org;
				}
				tQuote += "</cite>\n";
				tQuote += "</blockquote>\n";
			}
		}
		
		this.target.innerHTML = tQuote;
	},
	getRandomId: function() {
		return (Math.floor(Math.random() * this.testimonialsList.length));		
	},
	
	showOne: function(){
		var i, tQuote = "", text, name, title, org;
				
		i = this.getRandomId();

		while(i == this.lastQuoteId) {
			i = this.getRandomId();
		}

		this.lastQuoteId = i;
		
		text = this.testimonialsList[i].text || "";
		name = this.testimonialsList[i].name || "";
		title = this.testimonialsList[i].title || "";
		org = this.testimonialsList[i].organization || "";

		if (text != "" || name != "") {
			tQuote = "<blockquote class='single'>\n";
			tQuote += "<p>\n";
			tQuote += this.testimonialsList[i].text;
			tQuote += "</p>\n<cite>-- ";
			tQuote += name;
			if (title != "") {
				tQuote += ", " + title;
			}
			if (org != "") {
				tQuote += "<br />" + org;
			}
			tQuote += "</cite>\n";
			tQuote += "</blockquote>\n";
			
			this.target.innerHTML = tQuote;				
		}			
	},
	
	rotateOne: function() {	
		var that = this;
		var anim = new YAHOO.util.Anim( that.target, 
				{opacity: { from: 1, to: 0}}, 
				1, YAHOO.util.Easing.easeOut);
		
		anim.animate();

		anim.onComplete.subscribe( function(){
			that.showOne();
			var anim = new YAHOO.util.Anim(that.target, 
					{opacity: { from: 0, to: 1}}, 
					1, YAHOO.util.Easing.easeOut);
			anim.animate();
			setTimeout(function() {that.rotateOne();}, "15000");
		});
	},
	
	setRotationInterval: function(seconds) {
		if(typeOf (seconds === "number")) {
			this.rotationInterval = seconds * 1000; // convert to miliseconds
		}
	},
	
	setTestimonialsList: function(list) {
		if(list) {
			this.testimonialsList = list;
		}
	}
};

Tawala.testimonials.list = [
	{
		text: "You've made me look like a genius for merely suggesting the Dashboard and putting it in place.  So I'll keep it that way and enjoy the limelight...",
		name: "Ray Buddie",
		organization: "Central Marin Chargers"
	},
	{
		text: "...the registration process was great and we brought in a lot of new players.",
		name: "Sam Hickerson",
		organization: "Lomita Little League"
	},
	{
		text: "The program is awesome! It has really made a tremendous difference in the communication" + 
				" process between St. Hilary CYO &amp; our coaches... and the parents/players. Registration" + 
				" was much easier for everyone, and it will continue in the years to follow.",
		name: "Kevin Finn", 
		title: "Athletic Director", 
		organization: "St. Hilary's CYO Basketball"
	},
	{
		text: "The dashboard makes communicating with my team and parents fast and efficient. It's simple" + 
				" and it works!",								
		name: "Martin Gamboa", 
		title: "Coach",
		organization: "Mill Valley Little League"
	},
	{
		text: "Just wanted to say last year it took me DAYS to compile the uniform info and double check...this year less than 1/2 hour. Thanks! (and my family thanks you!)", 
		name: "Renee Gamble", 
		title: "Registrar", 
		organization: "San Rafael Girls Softball"
	},
	{
		text: "I am coaching my daughter's softball team this year and they have introduced the Tawala-Sports Dashboards. I absolutely love it. It is the perfect tool to communicate with. It is very simple and straight forward. My assistant coaches love it too!", 
		name: "Kelli Cook", 
		title: "Coach", 
		organization: "Tiburon Girls Softball"
	},
	{
		text: "I wanted to compliment you on the registration process.  Very easy, very fast. Great job.", 
		name: "Stephanie Dorfman", 
		title: "Parent", 
		organization: "Mill Valley Little League"
	},
	{
		text: "This is an extremely user friendly program.  I am totally happy with it and it has saved me countless hours of work!  I was pretty picky with Steve about what I wanted and he was incredibly responsive to my requests. Once our registration started, I thought I would be calling him more, but I only had a few small questions and he was able to help me out right away. I have had lots of favorable response from the parents.", 
		name: "Nancy Hecht", 
		organization: "St. Hilary CYO Basketball"
	},
	{
		text: "The Tawala registration and payment system has been a HUGE benefit to the rugby club (and also to the basketball foundation).  It has been easy to set up and work with.  And the time saved by not chasing papers and checks has been massive.  The club manager, registrar and treasurer think I'm a genius or a godsend or both.  Since I am neither, it must be your system!  Please consider me a reference for any prospective customers.",
		name: "Keith Milne", 
		title: "President", 
		organization: "Piedmont Rugby Club, Piedmont Basketball Foundation"
	},
	
	{
		text: "Thanks to you and Tawala our Fall Ball program is light years ahead of where it stood last year.  I have had mountains of positive feedback from coaches and players alike regarding the dashboard format, ease of communication, etc.  After this Fall Ball tryout NOTHING could replace TAWALA !!!  It is, hands down, the best service and support I could ever imagine.", 
		name: "Steve Bouchard", 
		title: "Commissioner", 
		organization: "Mill Valley Little League"
	},
	{
		text: "I wish everything were this easy.  And reasonable.",  
		name: "Lisa Scarsella", 
		organization: "Mill Valley Girls Softball"
	}
];


YAHOO.util.Event.onDOMReady(Tawala.testimonitalsInit);
