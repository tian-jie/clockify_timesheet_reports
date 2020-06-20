(function () {
	/**
	 * Created by Json on 2015/1/8.
	 */
	function onTap( el, func, isStopPropagation ) {
		el.style.setProperty( "cursor", "pointer", null );
		var dx, dy;
		var down = function ( downe ) {
			isStopPropagation && downe.stopPropagation();
			dx = downe.pageX;
			dy = downe.pageY;
			el.classList.add( "tap" );
			var move = function ( movee ) {
			};
			var end = function ( e ) {
				el.classList.remove( "tap" );
				document.removeEventListener( "mousemove", move, false );
				document.removeEventListener( "mouseup", end, false );
				if ( e.timeStamp - downe.timeStamp < 500 && Math.pow( (e.pageX - downe.pageX), 2 ) + Math.pow( (e.pageY - downe.pageY), 2 ) < 9 && e.target == el ) {

				    func && func();
				}
			};
			document.addEventListener( "mousemove", move, false );
			document.addEventListener( "mouseup", end, false );
		};
		el.addEventListener( "mousedown", down, false );
	}

	// 长按
	function onLTap( el, func, isStopPropagation ) {
		el.style.setProperty( "cursor", "pointer", null );
		var timeID;
		el.on( "mousedown", function ( downe ) {
			var mx = downe.pageX, my = downe.pageY;
			timeID = setTimeout( function () {
				el.classList.toggle( "select" );
				if ( Math.pow( downe.pageX - mx, 2 ) + Math.pow( downe.pageY - my, 2 ) < 9 ) {
					func && func();
				}
			}, 1000 );
			var move = document.on( "mousemove", function ( m ) {
				mx = m.pageX;
				my = m.pageY;
			} );
			isStopPropagation && downe.stopPropagation();
			var up = document.on( "mouseup", function ( e ) {
				up.destroy();
				move.destroy();
				clearTimeout( timeID );
			}, false );
		}, false );
	}

	function loop( num, func ) {
		for ( var i = 0; i < num; i++ ) {
			func( i );
		}
	}

	function loopArray( array, func ) {
		var len = array.length;
		for ( var i = 0; i < len; i++ ) {
			func( array[i], i );
		}
	}

	// 遍历对象
	function loopObj( obj, block ) {
		for ( var key in obj ) {
			block( key, obj[key] );
		}
	}

	function CSS( el, rules ) {
		loopObj( rules, function ( key, value ) {
			el.style.setProperty( key, value, null );
		} );
	}

	// 操作数组
	function reduce( initValue, array, operate ) {
		loopArray( array, function ( item, i ) {
			var result = operate( initValue, item, i );
			if ( result !== undefined ) {
				initValue = result;
			}
		} );
		return initValue;
	}

	// 判断对象类型
	var is = reduce( {}, ["Array", "Boolean", "Date", "Function", "Number", "Object", "RegExp", "String", "Window", "HTMLDocument"], function ( is, typeName ) {
		is[typeName] = function ( obj ) {
			return Object.prototype.toString.call( obj ) == "[object " + typeName + "]";
		};
	} );

	// 双向链表
	function LinkedList() {
		var head = null, tail = null;
		var count = 0;

		function addTail( value ) {
			var node = Node( value );
			node.previous = tail;

			if ( tail === null ) {
				head = node;
			}
			else {
				tail.next = node;
			}
			tail = node;

			return node;
		}

		function Node( value ) {
			var node = {
				previous : null,
				next : null,
				value : value,
				remove : function () {
					--count;
					if ( node.previous !== null ) {
						node.previous.next = node.next;
					}
					else {
						head = node.next;
					}

					if ( node.next !== null ) {
						node.next.previous = node.previous;
					}
					else {
						tail = node.previous;
					}
				},
				insertBefore : function ( value ) {
					var newNode = Node( value );
					newNode.previous = node.previous;
					newNode.next = node;
					node.previous = newNode;
					if ( newNode.previous !== null ) {
						newNode.previous.next = newNode;
					}
					else {
						head = newNode;
					}
					return newNode;
				},
				insertAfter : function ( value ) {
					return node.next === null ? addTail( value ) : node.next.insertAfter( value );
				}
			};

			++count;
			return node;
		}

		return {
			addTail : addTail,
			addHead : function ( value ) {
				return head === null ? addTail( value ) : head.insertBefore( value );
			},
			head : function () {
				return head;
			},
			tail : function () {
				return tail;
			},
			count : function () {
				return count;
			}
		};
	}

	// 添加CSS规则
	var insertCSSRule = function () {
		var userSheet = LinkedList(), systemSheet = LinkedList();
		return function ( ruleText, isSystem ) {
			var styleSheet = isSystem ? systemSheet : userSheet; // 选择样式链表

			// 如果节点尚未创建,创建节点,系统样式表在所有样式表的最前,用户样式表在所有样式表的最后
			if ( styleSheet.node === undefined ) {
				styleSheet.node = document.head.insertBefore( document.createElement( "style" ), isSystem ? document.head.firstChild : null );
			}

			// 创建新规则,位置上最后规则+1
			var lastRule = styleSheet.tail(),
				newRule = styleSheet.addTail( lastRule === null ? 0 : lastRule.value + 1 );

			styleSheet.node.sheet.insertRule( ruleText, newRule.value );

			return {
				remove : function () {
					// 后面所有元素的位置-1
					var pos = newRule.value;
					for ( var curNode = newRule.next; curNode !== null; curNode = curNode.next ) {
						curNode.value = pos++;
					}

					// 移除节点并删除规则
					newRule.remove();
					styleSheet.node.sheet.deleteRule( pos );
				}
			};
		}
	}();

	// 生成CSS样式字符串
	function cssRuleString( cssStyles ) {
		var ruleText = "";
		loopObj( cssStyles, function ( styleName, styleValue ) {
			function addRule( styleName ) {
				ruleText += styleName + ":" + styleValue + ";";
			}

			styleName in nonstandardStyles ? loopArray( nonstandardStyles[styleName], addRule ) : addRule( styleName );
		} );
		return ruleText;
	}

	function insertCSSRules( arg1, arg2, arg3 ) {
		function insertRules( selector, styles, isSystem ) {
			insertCSSRule( selector + " {" + cssRuleString( styles ) + "}", isSystem );
		}

		if ( is.String( arg1 ) ) {
			insertRules( arg1, arg2, arg3 );
		}
		else {
			loopObj( arg1, function ( selector, styles ) {
				insertRules( selector, styles, arg2 );
			} );
		}
	}

	function element( tagName, arg, parentNode ) {
		var el = document.createElement( tagName );
		arg && loopObj( arg, function ( key, value ) {
			switch ( key ) {
				case "classList":
					if ( typeof value == "string" ) {
						el.classList.add( value );
					}
					else {
						loopArray( value, function ( className ) {
							el.classList.add( className );
						} );
					}
					break;
				case "src":
					el.src = value;
					break;
				case "style":
					CSS( el, value );
					break;
				default:
					el[key] = value
			}
		} );
		parentNode && parentNode.appendChild( el );
		return el;
	}

	var Insert = function ( fatherObj, childObj ) {
		var key;
		for ( key in childObj ) {
			fatherObj[key] = childObj[key];
		}
	};

	var message = function () {
		var msgBox = $( ".fp-msg-box" );
		if ( msgBox.length == 0 ) {
			msgBox = $( "<div class='fp-msg-box'></div>" );
			msgBox.css( {
				position : "absolute",
				height : "36px",
				"line-height" : "36px",
				top : "0",
				left : "50%",
				"-webkit-transform" : "translate3d(-50%,0,0)",
				padding : "0 35px",
				color : "white",
				"font-size" : "14px",
				"border-radius" : "0 0 5px 5px",
				"background-color" : "rgba(0, 0, 0, 0.8)",
				"z-index" : "100000",
				"-moz-box-shadow" : "0 2px 4px rgba(0, 0, 0, 0.6)",
				"-webkit-box-shadow" : "0 2px 4px rgba(0, 0, 0, 0.6)",
				"box-shadow" : "0 2px 4px rgba(0, 0, 0, 0.6)"
			} );
			msgBox.hide();
			$( "body" ).append( msgBox );
		}

		return function ( options ) {
			var msg = $( "<div class='fp-msg'></div>" );
			msg.css( {
				position : "relative",
				"margin-bottom" : "10px"
			} );
			msg.addClass( options.type || "" );
			msg.text( options.text );
			msgBox.fadeIn();
			// 先清空
			msgBox[0].innerHTML = "";
			msgBox.append( msg );
			function removeMsg() {
				msgBox.fadeOut( "normal", function () {
					msgBox.html( "" );
				} );
				msgBox.unbind( "click" );
				clearTimeout( timeoutRemove );
			}

			msgBox.bind( "click", removeMsg );
			var timeoutRemove = setTimeout( removeMsg, 3000 );
			return msgBox;
		};
	}();

	Node.prototype.on = function ( eventType, handler, useCapture ) {
		var el = this;
		if ( !el.addEventListener ) {
			return;
		}
		this.addEventListener( eventType, handler, useCapture || false );
		return {
			destroy : function () {
				el.removeEventListener( eventType, handler, useCapture );
			}
		}
	};

	function translate( el, left, top, scale, rotate, duration, onEnd ) {
		el.x = left;
		el.y = top;
		el.startX = left;
		el.startY = top;
		CSS( el, {
			"transform" : "translate3d(" + left + "px," + top + "px,0px) scale(" + (scale || el.wscale || 1) + ") rotate(" + (rotate || el.wrotate || 0) + "deg)",
			"-webkit-transform" : "translate3d(" + left + "px," + top + "px,0px) scale(" + (scale || el.wscale || 1) + ") rotate(" + (rotate || el.wrotate || 0) + "deg)"
		} );
		scale && (el.wscale = scale);
		rotate && (el.wrotate = rotate);
		duration && CSS( el, {
			"transition" : duration + "s"
		} );
		var endType = ["transitionend", "webkitTransitionEnd"];
		loopArray( endType, function ( type ) {
			var end = {};
			end[0] = el.on( type, function () {
				el.style.removeProperty( "transition" );
				el.style.removeProperty( "-webkit-transition" );
			} );
			if ( onEnd ) {
				end[1] = el.on( type, onEnd );
				el.on( type, function () {
					end[1].destroy();
				} );
			}
		} );
	}

	function onDrag( dragEl, events ) {
		dragEl.on( "mousedown", function ( down ) {
			down.stopPropagation();
			var isDrag = false;
			dragEl.wx == undefined && (dragEl.wx = 0);
			dragEl.wy == undefined && (dragEl.wy = 0);
			var moveHandle = document.on( "mousemove", function ( move ) {
				if ( !isDrag && Math.pow( move.pageX - down.pageX, 2 ) + Math.pow( move.pageY - down.pageY, 2 ) > 9 ) {
					isDrag = true;
				}
				isDrag && events.move && events.move( {
					dx : move.pageX - down.pageX,
					dy : move.pageY - down.pageY
				} );
			} );
			var upHandle = document.on( "mouseup", function ( up ) {
				dragEl.wx += up.pageX - down.pageX;
				dragEl.wy += up.pageY - down.pageY;
				moveHandle.destroy();
				upHandle.destroy();
				isDrag && events.end( {
					dx : up.pageX - down.pageX,
					dy : up.pageY - down.pageY
				} );
			} );
		} );
	}

	function scaleAndRotate( el, initX, initY, obj ) {
		var border = el.parentNode;
		var initDs = Math.sqrt( Math.pow( initX, 2 ) + Math.pow( initY, 2 ) );
		el.on( "mousedown", function ( down ) {
				border.wscale = border.wscale || 1;
				border.wrotate = border.wrotate || 0;
				el.wx = initDs * border.wscale * Math.sin( border.wrotate * 2 * Math.PI / 360 );
				el.wy = initDs * border.wscale * Math.cos( border.wrotate * 2 * Math.PI / 360 );
				down.stopPropagation();
				down.preventDefault();
				var moveHandle = document.on( "mousemove", function ( move ) {
					// 鼠标的距離
					var x = move.pageX - down.pageX + el.wx;
					var y = -(move.pageY - down.pageY) + el.wy;
					// 旋轉的角度
					var rotate = Math.atan( x / y ) / (2 * Math.PI) * 360;
					if ( x > 0 && y < 0 || x < 0 && y < 0 ) {
						rotate = 180 + rotate;
					}
					border.wrotate = rotate;
					var ds = Math.sqrt( Math.pow( x, 2 ) + Math.pow( y, 2 ) );
					var scale = ds / initDs;
					border.wscale = scale;
					CSS( border, {
						"-webkit-transform" : "translate3d(" + (border.wx || 0) + "px," + (border.wy || 0) + "px,0) scale(" + scale + ") rotateZ(" + rotate + "deg)"
					} );
					obj.move && obj.move( {
						scale : scale,
						rotate : rotate
					} );
				} );
				var upHandle = document.on( "mouseup", function ( up ) {
					obj.end && obj.end();
					el.wx += up.pageX - down.pageX;
					el.wy += -(up.pageY - down.pageY);
					moveHandle.destroy();
					upHandle.destroy();
				} )
			}
		)
		;
	}

	function scale( el, initX, initY, obj ) {
		var rawds = Math.sqrt( Math.pow( initX, 2 ) + Math.pow( initY, 2 ) );
		var border = el.parentNode;
		var rawd = Math.atan( initX / initY );
		el.on( "mousedown", function ( down ) {
			border.wscale = border.wscale || 1;
			el.wx = rawds * border.wscale * Math.sin( border.wrotate * 2 * Math.PI / 360 + rawd );
			el.wy = rawds * border.wscale * Math.cos( border.wrotate * 2 * Math.PI / 360 + rawd );
			down.stopPropagation();
			down.preventDefault();
			var moveHandle = document.on( "mousemove", function ( move ) {
				// 鼠标的距離
				var x = -(move.pageX - down.pageX) + el.wx;
				var y = move.pageY - down.pageY + el.wy;
				var ds = Math.sqrt( Math.pow( x, 2 ) + Math.pow( y, 2 ) );
				var scale = ds / rawds;
				border.wscale = scale;
				CSS( border, {
					"-webkit-transform" : "translate3d(" + (border.wx || 0) + "px," + (border.wy || 0) + "px,0) scale(" + scale + ") rotateZ(" + (border.wrotate || 0) + "deg)"
				} );
				obj.move && obj.move( {
					scale : scale,
					rotate : border.wrotate
				} );
			} );
			var upHandle = document.on( "mouseup", function ( up ) {
				el.wx += up.pageX - down.pageX;
				el.wy += -(up.pageY - down.pageY);
				obj.end && obj.end();
				moveHandle.destroy();
				upHandle.destroy();
			} )
		} );
	}

	window.fp == undefined && (window.fp = {});

	Insert( fp, {
		onTap : onTap,
		loop : loop,
		loopArray : loopArray,
		loopObj : loopObj,
		CSS : CSS,
		element : element,
		message : message,
		translate : translate,
		onDrag : onDrag,
		onLTap : onLTap,
		scale : scale,
		scaleAndRotate : scaleAndRotate,
		insertCSSRules : insertCSSRules
	} );
})();