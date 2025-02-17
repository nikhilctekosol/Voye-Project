$(document).ready(function(){
	let menuIcon = document.querySelector(".menu-icon");
	let lines = Array.from(menuIcon.children);

	function toggleMenu(e) {
	    lines.forEach(line => {
	        menuIcon.classList.toggle("active");
	        line.classList.toggle("active");
	        line.classList.toggle("no-animation");
	    });
	}

	menuIcon.addEventListener("click", toggleMenu);
});

$(document).ready(function(){
	const sections = document.querySelectorAll("section");

	[...sections].forEach((section) => {
	  const checkbox = section.querySelector("input");

		if (checkbox) {
			checkbox.addEventListener("change", () => {
				section.classList.toggle("enable-animation");
			});
		}
	});
});

$(document).ready(function(){
	const lightbox = GLightbox({
		touchNavigation: true,
		loop: true,
		width: "90vw",
		height: "90vh"
	});
});
  
$(document).ready(function(){
function moreLess(initiallyVisibleCharacters) {
	var visibleCharacters = initiallyVisibleCharacters;
	var paragraph = $(".text")
	
  
	paragraph.each(function() {
	  var text = $(this).text();
	  var wholeText = text.slice(0, visibleCharacters) + "<span class='ellipsis'>... </span><a href='#' class='more'>Read More<i class='fa fa-arrow-circle-o-down' aria-hidden='true'></i></a>" + "<span style='display:none'>" + text.slice(visibleCharacters, text.length) + "<a href='#' class='less'> Read Less<i class='fa fa-arrow-circle-o-up' aria-hidden='true'></i></a></span>"
	  
	  if (text.length < visibleCharacters) {
		return
	  } else {
		$(this).html(wholeText)
	  }
	});
	$(".more").click(function(e) {
	  e.preventDefault();
	  $(this).hide().prev().hide();
	  $(this).next().show();
	});
	$(".less").click(function(e) {
	  e.preventDefault();
	  $(this).parent().hide().prev().show().prev().show();
	});

	$(".see-more").click(function (event) {
		$(".see-more").hide();
		$(".see-less").show();
		$(".about-property").addClass("about-property-full");
	});
	$(".see-less").click(function (event) {
		$(".see-more").show();
		$(".see-less").hide();
		$(".about-property").removeClass("about-property-full");
	});
  };
  
  moreLess(200);
});


$(document).ready(function() {

	var adultCount = $('#adultsCount');
	var childCount = $('#childrenCount');

	$('#a-cnt-up').click(function () {
		adultCount.val(Math.min(parseInt($('#adultsCount').val()) + 1, 20));
	});
	$('#a-cnt-down').click(function () {
		adultCount.val(Math.max(parseInt($('#adultsCount').val()) - 1, 1));
	});
	$('#c-cnt-up').click(function () {
		childCount.val(Math.min(parseInt($('#childrenCount').val()) + 1, 20));
	});
	$('#c-cnt-down').click(function () {
		childCount.val(Math.max(parseInt($('#childrenCount').val()) - 1, 1));
	});
  
	//$('.btn').click(function() {
  
	//  var $btn = $('.btn');
  
	//  $btn.toggleClass('booked');
	//  $('.diamond').toggleClass('windup');
	//  $('form').slideToggle(300);
	//  $('.linkbox').toggle(200);
  
	//  if ($btn.text() === "BOOK NOW") {
	//	$btn.text("Change Date");
	//  }
	//});
});

$(document).ready(function() {
	var menuLeft = document.getElementById( 'cbp-spmenu-s1' ),
	showLeftPush = document.getElementById( 'showLeftPush' ),
	body = document.body;

	showLeft.onclick = function() {
		classie.toggle( this, 'active' );
		classie.toggle( menuLeft, 'cbp-spmenu-open' );
		disableOther( 'showLeft' );
	};
	showLeftPush.onclick = function() {
		classie.toggle( this, 'active' );
		classie.toggle( body, 'cbp-spmenu-push-toright' );
		classie.toggle( menuLeft, 'cbp-spmenu-open' );
		disableOther( 'showLeftPush' );
	};
	function disableOther( button ) {
		if( button !== 'showLeft' ) {
			classie.toggle( showLeft, 'disabled' );
		}
		if( button !== 'showLeftPush' ) {
			classie.toggle( showLeftPush, 'disabled' );
		}
	}
});

//function initMap(lat, lng) {

//	// The location of pos
//	var pos = { lat: lat, lng: lng };
//	// The map, centered at Uluru
//	var map = new google.maps.Map(
//		document.getElementById('map'), { zoom: 13, center: pos, disableDefaultUI: true, fullscreenControl: true });
//	// The marker, positioned at Uluru
//	var marker = new google.maps.Marker({ position: pos, map: map });
//}

$(document).ready(function () {
	var owl = $('.owl-carousel');
	owl.owlCarousel({
		stagePadding: 25,
		items: 1,
		loop: true,
		margin: 10,
		autoplay: true,
		autoplayTimeout: 3000,
		autoplayHoverPause: true,
		dots: true,
		responsive: {
			620: {
				items: 2,
				stagePadding: 50,
				autoplay: true
			},
			992: {
				stagePadding: 75,
				items: 3,
				autoplay: true
			}
		}
	});

	$(".owl-prev").click(function () {
		owl.trigger("prev.owl.carousel");
	});

	$(".owl-next").click(function () {
		owl.trigger("next.owl.carousel");
	});
});

$(document).ready(function () {
	function setMinStartDate() {
		const startDateInput = document.getElementById('startDate');
		const today = new Date().toISOString().split('T')[0];
		startDateInput.setAttribute('min', today);
	}

	function setMinEndDate() {
		const startDate = document.getElementById('startDate').value;
		const endDateInput = document.getElementById('endDate');

		if (startDate) {
			const minEndDate = new Date(startDate);
			minEndDate.setDate(minEndDate.getDate() + 1);
			const minEndDateFormatted = minEndDate.toISOString().split('T')[0];

			endDateInput.setAttribute('min', minEndDateFormatted);

			if (endDateInput.value < minEndDateFormatted) {
				endDateInput.value = minEndDateFormatted;
			}
		}
	}

	document.getElementById('startDate').addEventListener('focus', setMinStartDate);
	document.getElementById('startDate').addEventListener('change', setMinEndDate);
});


function isPhoneNumber(event) {
	const inputChar = String.fromCharCode(event.which);
	const currentInput = event.target.value;

	// Allow only numbers and one '+' at the start
	if (!/[0-9+ ]/.test(inputChar) && (inputChar !== '+' || currentInput.includes('+') || currentInput.length > 0)) {
		event.preventDefault();
		return false;
	}
	return true;
}