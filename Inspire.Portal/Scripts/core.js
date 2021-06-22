    //tab navigation
    $('.tab-nav').click(function(e) {
        $(this).addClass('active').siblings().removeClass('active');
        $($(this).attr('href')).addClass('active').siblings().removeClass('active');
        goToThis($(this).attr('href'), -70);
    });

    //CAROULSEL index page
    if($('.providers-carousel').length) {
        $('.providers-carousel').owlCarousel({
            responsive: {
                0: { items: 1 },
                768: { items: 2 },
                980: { items: 3 },
                1080: { items: 4 }
            },
            nav: true,
            autoplay:true,
            autoplayTimeout:5000,
            slideSpeed : 300,
            paginationSpeed : 400,
            loop:true,
            navText: [
                '<svg class="icon r90"><use xlink:href="#icon-arrowdown"></use></svg>',
                '<svg class="icon r270"><use xlink:href="#icon-arrowdown"></use></svg>'
            ]
        });
    }

    //GENERAL ERROR POPUP
    function genpop(a,b,c) {
        //content, customclass, title
        $('#genpop').remove();
        if (typeof(event) !== 'undefined')
            event.preventDefault();
        if (c != undefined && c != '') {
            switch(b){
                case 'success': c = 'Success';break;
                case 'warning': c = 'Warning';break;
                case 'note':    c = 'Note';break;
                case 'error':   c = 'Error';break;
                default: break;
            }
        }
        c = '<div id="genpop" class="'+b+'"><div class="genbody"><div class="icon"></div><div>'+((c == '')? '':('<h6>'+c+'</h6>'))
        +'<div class="p ctrl">'+a+'</div><a href="#" class="_close">x</a></div></div></div>';
        $('body').append(c);
        $('input,textarea').blur();
    }

    //ESCAPE KEY
    $(document).keyup(function(e) {
        if (e.keyCode == 27) {
            $('#genpop ._close').click();   // esc
            $('#respmenu').prop('checked', false)
            $('body').removeClass('openedmenu openedsearch')
        }
    });

    //FUTURE RESPONSIVE
    $('#respmenu').on('click',function(e){
        $('body').toggleClass('openedmenu');
    });
    
    //const ps = new PerfectScrollbar('.responsivenav nav');

    //CLOSING GENPOP
    $('body').on('click','#genpop ._close',function(e){
        e.preventDefault();
        $('#genpop').fadeOut(300);
        setTimeout(function(){
            $('#genpop').remove();
        }, 300);
    }); //$('body').removeClass('overflow-hidden')

    

    //SLIDE TO ELEMENT class and function
    function goToThis(a,z){
        //console.log('a is '+a);
        if(z === undefined)
            z = 0;
        if (a !== false) {
            var b = 0;            
            if(a !== undefined && a !== '' && a != '#') {b = $(a).offset().top;}
            $('html,body').animate({scrollTop:b+z},600)
        }
    }
    
    $('body').on('click','.gotothis', function(e){
        e.preventDefault();
        var v = $(this).attr('href');
        if (v === undefined || v == '') {
            v = $(this).data('gotothis');
        }
        if (v == 'javascript:;' || v == 'false') {v = false;}
        goToThis(v);
    });

    $('.settings').magnificPopup({
        type: 'inline'
    });

    //detect TOUCHscreen 
    $('body').addClass('touch_'+(Boolean('ontouchstart' in window || navigator.maxTouchPoints)+''));
    
    
    //FIXATE MENU
    var fixat = $('.header-bottom').offset().top;
    $(window).scroll(function(){
        //console.log($(this).scrollTop());
        if ($(this).scrollTop() > fixat && !$('body').hasClass('fixed')) {
            $('body').addClass('fixed');
        } else if ($(this).scrollTop() <= fixat && $('body').hasClass('fixed')){
            $('body').removeClass('fixed');
        }
        var asideHeight = $('.content-pre-wrap ul').height();
        //console.log(asideHeight);
        if ($(window).width() > 768) {
            if ($(this).scrollTop() > 200
            && asideHeight < $('.content-wrap').height()
            && asideHeight < $(window).height()
            && (asideHeight + $(this).scrollTop() - 107) < $('.content-pre-wrap').height()) {
                $('aside').css({
                    'top': ($(this).scrollTop() - 170) + 'px'
                })
            } else if ($(this).scrollTop() < 200) {
                $('aside').attr('style', '');
            }
        }
    }).trigger('scroll');


    //-----------   A+A-A FONT CONTROL  ---------------------------//
    //set cookie
    function setCookie(cname, cvalue, exdays) {
        if(!getCookie('cookiesgdpr')) {
            genpop(resources.AcceptTermsAndConditionsMessage,'error','GDPR');
            return false;
        }
        var d = new Date();
        d.setTime(d.getTime() + (exdays*24*60*60*1000));
        var expires = "expires="+d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires  + "; path=/";
    }
    //get cookie
    function getCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for(var i=0; i<ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0)==' ') c = c.substring(1);
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    }
    //check for cookie, if there is one, set body
    var check = getCookie("text");
    /*
        if (check != "") {
            $('body').addClass(check);
        }
    */

    //list of all classes
    var classes = ['t08','t09','t00','t15','t16','t18','t20','t22'];

    //control over website
    $('.text-up, .text-down').on('click',function(e){
        e.preventDefault();
        var direction = ($(this).hasClass('text-up'))? true:false, //up and down, true and false
            flag = false,
            getindex = '',
            currentClasses = $('body').attr('class').split(' ');
        for (var i=0; i < classes.length;i++) {
            if($.inArray(classes[i],currentClasses) > -1) {
                getIndex = $.inArray(classes[i],currentClasses);
                if (direction) {
                    getClass = classes[i+1];
                } else {
                    getClass = classes[i-1];
                }
                flag = false;
                break;
            } else {
                flag = true;
            }
        }
        if (flag) {
            if (direction) {
                $('body').addClass('t15');
                getClass="t15";
            } else {
                $('body').addClass('t09');
                getClass="t09";
            }
        } else {
            currentClasses.splice(getIndex, 1);
            currentClasses.push(getClass);
            $('body').attr('class',currentClasses.join(' '));
        }
        //remember the choice, for a while
        setCookie('text', getClass, 7);
    });

    //reset cookie and font
    $('.reset_text').on('click',function(e){
        e.preventDefault();
        $('body').removeClass('t05 t06 t07 t08 t09 t00 t11 t12 t13 t14 t15 t16 t18 t20 t22');
        setCookie('text', '', -7);
    });

    //-----------   END A+A-A   ---------------------------//


    //NO STYLES


    //var b = true; set from php/code
    //EXAMPLE:  <script type="text/javascript">var b = {if !cookies.blind}false{else}true{/if}</script>
    $('#blind,.reset-blind').on('click',function(e){
        e.preventDefault();
        if (!$(this).hasClass('reset-blind')) {
            for ( i=0; i<document.styleSheets.length; i++) {
                //void(document.styleSheets.item(i).disabled=true);
                $('link').prop('disabled',true);
            }
            $('link[data-keep-alive="true"]').attr('disabled',false);
            setCookie('blind', 'true', 7);
            $('body').addClass('blind');
            b = false;
        } else {
            for ( i=0; i<document.styleSheets.length; i++) {
                //void(document.styleSheets.item(i).disabled=false);
                $('link').attr('disabled',false);
            }
            $('body').removeClass('blind');
            b = true;
            setCookie('blind', '', -7);
        }
    });


    //TOGGLE COLOR STYLES
    $('#style, .style-change').on('click',function(e){
        e.preventDefault();
        if($(this).data('style') !== undefined) {
            $('body').removeClass('bw wb gs gb md').addClass($(this).data('style'))
            setCookie('color', $(this).data('style'), 7);
        } else if($('body').hasClass('bw')) {
            $('body').removeClass('bw').addClass('wb')
            setCookie('color', 'wb', 7);
        } else if($('body').hasClass('wb')) {
            $('body').removeClass('wb bw').addClass('gs');
            setCookie('color', 'gs', 7);
        } else if($('body').hasClass('gs')) {
            $('body').removeClass('wb bw gs').addClass('gb');
            setCookie('color', 'gb', 7);
        } else if($('body').hasClass('gb')) {
            $('body').removeClass('wb bw gs gb').addClass('md');
            setCookie('color', 'md', 7);
        } else if($('body').hasClass('md')) {
            $('body').removeClass('wb bw gs gb md');
            setCookie('color', '', 7);
        } else {
            $('body').addClass('bw');
            setCookie('color', 'bw', 7);
        }
    });
    
    $('body').on('click','.btn-with-content .dropdown-trigger',function(e){
	    e.stopImmediatePropagation();
	    e.stopPropagation();
	    e.preventDefault();
	    var target = $(e.target);
	    //console.log(target);
	    if ((target.hasClass("dropdown-trigger") ||  target.hasClass("icon currentColor"))
	    && !target.parents('.btn-with-content').hasClass('openeddropdown')
	    ) {
	        e.preventDefault();
	        e.stopPropagation();
	        $('.openeddropdown').removeClass('openeddropdown');
	        target.parents('.btn-with-content').addClass('openeddropdown');
	        var overflowcompensation = target.parents('.overwrite-table');
	        overflowcompensation.attr('style', '');
	        if (overflowcompensation.get(0).clientHeight != overflowcompensation.get(0).scrollHeight)
	            overflowcompensation.css({paddingBottom:(target.parents('.btn-with-content').find('ul').innerHeight()-53)+'px'})
	    } else {
	        $('.openeddropdown').removeClass('openeddropdown');
	        $('.overwrite-table').attr('style', '');
	    }
	});
	
	
	$('.hasdropdown > a').on('click',function(e){
		if(window.innerWidth < 1280) {
		    e.preventDefault();
		    $(this).parent().toggleClass('active');		
		    console.log(1);	
		    return false;
		}	    
	});
