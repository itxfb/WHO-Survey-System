(function($){
	$.fn.ublue_ImageCarousel=function(opts){
		var $this = $(this);
		// options
		opts = $.extend({
			// Dom
			ubArea:'.item-preview',				
			ubCon:'.focusCon',					
			ubItem:'.carouselItem',				
			ubIndicators:'.carouselIndicators',		
			ubPrev:'.focusPrev',					
			ubNext:'.focusNext',				
			ubEffect:'left',						
			ubTrigger:'click',					
			ubAutoPlay:'off',					
			ubAutoBtn:'off',						
			ubTitleHover:'off',					
			ubGallery:'off',						
			ubTime:5000,						
			ubSpeed:480,						
			ubTitleSpeed:50					
		}, opts);
		var $ubDistance,
			$ubJudge,
			$ubMax,
			$ubItemSize,
			$stpe			= 0,
			$animate		= {},$direction,
			$ubCon		= $this.find(opts.ubCon),
			$ubItem		= $this.find(opts.ubItem),
			$ubIndicators	= $this.find(opts.ubIndicators),
			$ubCount		= $ubItem.length,
			$ubItemW		= $ubItem.outerWidth(true),
			$ubItemH		= $ubItem.outerHeight(true),
			$ubMaxW		= Math.floor( $ubCon.width()/$ubItem.width() ),
			$ubMaxH		= Math.floor( $(this).find(opts.ubArea).height()/$ubItem.height() );
		// Initialization
		if( opts.ubEffect == 'fade' ){
			$ubMax = $ubMaxW;
			$ubItem.eq(0).show();
		}
		if( opts.ubEffect == 'left' || opts.ubEffect == 'leftSeamless' ){
			$ubItemSize = $ubItemW;
			$ubMax = $ubMaxW;
			$direction = 'left';
		}
		if( opts.ubEffect == 'top' || opts.ubEffect == 'topSeamless'){
			$ubItemSize = $ubItemH;
			$ubMax = $ubMaxH;
			$direction = 'top';
		}
		if ( opts.ubEffect == 'left' ) {
			$ubCon.css( 'width',$ubItemSize*$ubCount );
		}
		if ( opts.ubEffect == 'leftSeamless' ) {
			$ubCon.css( 'width',$ubItemSize*($ubMax+$ubCount) );
		}
		$ubDistance = opts.ubGallery=='off'?1:$ubMax;
		$ubJudge = opts.ubGallery=='off'?$ubCount-$ubMax:$ubCount/$ubMax-1;
		// Seamless
		if ( opts.ubEffect == 'leftSeamless' || opts.ubEffect == 'topSeamless' ) {
			var group = '<ul class=\'focusGroup\'></ul>';
			$ubCon.wrapInner(group).append(group);
			$ubCon.find('.focusGroup').last().append( $ubItem.slice(0, $ubMax).clone() );
			$ubJudge	= opts.ubGallery=='off'?$ubCount:Math.floor($ubCount/$ubMax);
		}
		// Indicators
		if ( opts.ubAutoBtn == 'on') {
			if ( opts.ubEffect == 'leftSeamless' || opts.ubEffect == 'topSeamless' ) {
				for ( var i =0; i < $ubJudge; i++ ) {
					$ubIndicators.append('<a>' + (i + 1) + '</a>');
				}
			}else{
				for ( var i =0; i <= $ubJudge; i++ ) {
					$ubIndicators.append('<a>' + (i + 1) + '</a>');
				}
			}
		}
		var $ubIndicatorsBtn = $ubIndicators.find('a');
		$ubIndicatorsBtn.eq(0).addClass('current');
		// Switch
		function effectSwitch(op){
			switch(opts.ubEffect){
				case 'fade':
					$ubItem.eq(op).fadeIn(opts.ubSpeed).siblings().fadeOut(opts.ubSpeed);
					break;
				default:
					$animate[$direction] = -op*$ubDistance*$ubItemSize;
					$ubCon.animate($animate,opts.ubSpeed);
			}
			indicatorsStyle($stpe);
		}
		function nextSwitch(){
			if ( !$ubCon.is(':animated') && !$ubItem.is(':animated') ) {
				if ( opts.ubEffect == 'leftSeamless' || opts.ubEffect == 'topSeamless' ){
					if ( $stpe == $ubJudge-1 ) {
						$stpe++;
						$animate[$direction] = -$stpe*$ubDistance*$ubItemSize;
						$ubCon.animate($animate,opts.ubSpeed);
						indicatorsStyle(0);
					}else if( $stpe == $ubJudge ){
						$stpe = 1;
						$ubCon.css($direction,'0');
						effectSwitch($stpe);
					}else{
						$stpe++;
						effectSwitch($stpe);
					}
				}else{
					if ( $stpe == $ubJudge ) {
						$stpe = 0;
						effectSwitch($stpe);
					}else{
						$stpe++;
						effectSwitch($stpe);
					}
				}
			}
		}
		function prevSwitch(){
			if ( !$ubCon.is(':animated') && !$ubItem.is(':animated') ) {
				if ( $stpe == '0' ) {
					if ( opts.ubEffect == 'leftSeamless' || opts.ubEffect == 'topSeamless' ){
						$ubCon.css( $direction,-$ubJudge*$ubDistance*$ubItemSize );
						$stpe = $ubJudge-1;
					}else{
						$stpe = $ubJudge;
					}
				}else{
					$stpe--;
				}
				effectSwitch($stpe);
			}
		}
		function indicatorsStyle(op){
			$ubIndicatorsBtn.eq(op).addClass('current').siblings().removeClass('current');
		}
		// Trigger
		$this.find(opts.ubPrev).click(prevSwitch);
		$this.find(opts.ubNext).click(nextSwitch);
		if ( opts.ubTrigger == 'hover' ) {
			$ubIndicatorsBtn.hover(function(e) {
				var i = $(this).index();
				triggerDelay = setTimeout(function() {
					$stpe = i;
					effectSwitch($stpe);
				}, 200);
			},function(){
				clearTimeout(triggerDelay);
			});
		}else{
			$ubIndicatorsBtn.click(function() {
				$stpe = $(this).index();
				effectSwitch($stpe);
			});
		}
	};
}(jQuery));
