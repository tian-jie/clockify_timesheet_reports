/**
 * Created by Json on 2014/12/31.
 */
(function () {
	window.fp = window.fp || {};

	//　包括个人设置
	fp.setting = {};

	function element( tag, property, parent ) {
		var el = document.createElement( tag );
		var key;
		for ( key in property ) {
			switch ( key ) {
				case "css":
					var cssname;
					for ( cssname in property[key] ) {
						el.style.setProperty( cssname, property[key][cssname], null );
					}
					break;
				default:
					el[key] = property[key];
			}
		}
		parent.appendChild( el );
		return el;
	}

	fp.setting.open = function ( obj ) {
		obj.editenable = true;
		var mask = element( "div", {
			"className" : "personal-setting-mask"
		}, document.body );
		var box = element( "div", {
			"className" : "personal-setting-box-border"
		}, document.body );
		var header = element( "div", {
			"className" : "personal-setting-box-header",
			innerHTML : "关联微信公众号"
		}, box );
		var tip = element( "div", {
			"className" : "personal-setting-box-tip"
		}, header );
		var tipText = element( "div", {
			"className" : "personal-setting-box-tip-text"
		}, box );
		tip.on( "mouseover", function () {
			tipText.classList.add( "show" );
		} );
		tip.on( "mouseout", function () {
			tipText.classList.remove( "show" );
		} );
		var closeButton = element( "div", {
			"className" : "personal-setting-box-close-btn"
		}, header );
		var line1 = element( "div", {
			"className" : "personal-setting-box-line"
		}, box );
		line1.classList.add( "line1" );
		var word1 = element( "div", {
			"className" : "personal-setting-box-word",
			innerHTML : "头像"
		}, line1 );
		var imgBorder = element( "div", {
			"className" : "personal-setting-box-img-border"
		}, line1 );
		var headImg = element( "img", {}, imgBorder );
		obj.img && (headImg.src = obj.img);
		headImg.onload = function () {
			if ( headImg.height > headImg.width ) {
				headImg.style.width = "100%";
			}
			else {
				headImg.style.height = "100%";
			}
		};
		var line2 = element( "div", {
			"className" : "personal-setting-box-line"
		}, box );
		line2.classList.add( "line2" );
		var word2 = element( "div", {
			"className" : "personal-setting-box-word",
			innerHTML : "名称"
		}, line2 );
		var inputName = element( "input", {
			className : "personal-setting-input",
			type : "text"
		}, line2 );

		inputName.onfocus = function () {
			inputName.placeholder = "一个月内只可更改一次，慎重修改";
		};
		inputName.onblur = function () {
			inputName.placeholder = "公众号名称";
		};

		inputName.placeholder = "公众号名称";
		var inputFile = element( "input", {
			type : "file",
			className : "hide",
			name : "headimg"
		}, line1 );
		var fileIcon = element( "div", {
			className : "personal-setting-file-icon"
		}, imgBorder );
		fileIcon.classList.add( "hide" );
		imgBorder.on( 'mouseover', function () {
			fileIcon.classList.remove( "hide" );
		} );
		imgBorder.on( 'mouseout', function () {
			fileIcon.classList.add( "hide" );
		} );
		var imgSizeOver = element( "span", {
			className : "personal-setting-image-over-size-message"
		}, line1 );

		var line3 = element( "div", {
			"className" : "personal-setting-box-line"
		}, box );
		line3.classList.add( "line3" );
		element( "div", {
			"className" : "personal-setting-box-word",
			innerHTML : "链接"
		}, line3 );
		var Url = element( "input", {
			className : "personal-setting-input",
			type : "text"
		}, line3 );
		Url.onfocus = function () {
			Url.placeholder = "一个月只能修改一次,慎重修改";
		};
		Url.onblur = function () {
			Url.placeholder = "可以配置一个关注公众号页面的链接";
		};
		Url.placeholder = "可以配置一个关注公众号页面的链接";

		var urlTip = element( "div", {
			className : "personal-setting-url-tips"
		}, line3 );
		urlTip.onclick = function () {
			window.open( "http://mp.weixin.qq.com/s?__biz=MzAwNDAzMjA3Nw==&mid=203559961&idx=1&sn=14b5eb18927aefeaa3134c8012d61341#rd", null, null, false );
		};

		var saveButton = element( "div", {
			className : "personal-setting-save-btn",
			innerHTML : '保存'
		}, box );

		mask.onclick = function () {
			document.body.removeChild( mask );
			document.body.removeChild( box );
		};
		closeButton.onclick = function () {
			document.body.removeChild( mask );
			document.body.removeChild( box );
		};

		fileIcon.onclick = function () {
			if ( obj.editenable && obj.editenable == true ) {
				inputFile.click();
			}
		};

		inputFile.addEventListener( "change", function () {
			var file = this.files[0];
			var KB = 1024;
			if ( file && file.size > 100 * KB ) {
				// 判断图片大小，如果超过100KB则提示图片过大
				imgSizeOver.innerHTML = "图片不能超过100KB";
			}
			else {
				imgSizeOver.innerHTML = "";
				var reader = new FileReader();
				reader.readAsDataURL( file );
				reader.onload = function () {
					headImg.src = this.result;
					headImg.onload = function () {
						if ( headImg.height > headImg.width ) {
							headImg.style.width = "100%";
						}
						else {
							headImg.style.height = "100%";
						}
					}
				}
			}
		}, false );
		if ( obj.editenable && obj.editenable == true ) {
			saveButton.onclick = function () {
				// 名称不能为空
				if ( $.trim( inputName.value ).length == 0 || $.trim( inputName.value ) == "名称不能为空" ) {
					inputName.value = "名称不能为空";
					return;
				}
				// 验证链接的合法性
				if ( Url.value.indexOf( "http://mp.weixin.qq.com" ) != 0 ) {
					fp.message( {
						text : "链接的格式不对"
					} );
					return;
				}
				var sendata = new window.FormData();
				sendata.append( "author", inputName.value );
				$.trim( Url.value ).length != 0 && sendata.append( "url", Url.value );
				var selectedFile = inputFile.files.length && inputFile.files[0];
				if ( selectedFile ) {
					sendata.append( "blob-fileName", selectedFile.name );
					sendata.append( "headimg", selectedFile, selectedFile.name );
				}
				fpInvokeAPI.saveUserSetting( sendata, function ( data ) {
					if ( !data.data ) {
						fp.message( {
							text : "一个月内只能设置一次，下个月再设置吧"
						} );
					}
					else {
						fp.message( {
							text : "修改成功"
						} );
					}
				} );
				document.body.removeChild( mask );
				document.body.removeChild( box );
			}
		}
		else {
			saveButton.title = "您已设置过，无法在编辑！";
		}
	};

	fp.changePasswordDialog = {};
	fp.changePasswordDialog.open = function () {
		if ( !window.userID ) {
			fpInvokeAPI.getUserInfo( function ( data ) {
				console.log( data );
				window.userID = data.data.ID;
				makeDialog();
			} );
		}
		else {
			makeDialog();
		}

		function makeDialog() {
			var mask = element( "div", {
				"className" : "personal-setting-mask"
			}, document.body );
			var box = element( "div", {
				"className" : "chuye-account-changepassword-box"
			}, document.body );

			var IDLabel = element( "span", {
				"className" : "chuye-account-id-label"
			}, box );
			IDLabel.innerHTML = window.userID;
			var closeButton = element( "div", {
				"className" : "chuye-change-password-close-btn"
			}, box );
			var makeSureBtn = element( "div", {
				"className" : "chuye-change-password-sure-btn"
			}, box );
			var inputName = element( "input", {
				className : "chuye-change-password-input",
				type : "password",
				placeholder : "密码长度6-16位"
			}, box );
			makeSureBtn.innerHTML = "确认";
			makeSureBtn.onclick = function () {
				var password = $.trim( inputName.value );
				if ( password.length < 6 || password.length > 16 ) {
					alert( "密码长度不符合要求" );
				}
				else {
					fpInvokeAPI.changePassword( password, function ( data ) {
						if ( data.code == 200 ) {
							fp.message( {
								text : "密码修改成功"
							} )
						}
						else {
							fp.message( {
								text : "密码修改失败"
							} )
						}
					} );
					close();
				}
			};
			mask.onclick = close;
			closeButton.onclick = close;
			function close() {
				document.body.removeChild( mask );
				document.body.removeChild( box );
			}
		}
	}
})();