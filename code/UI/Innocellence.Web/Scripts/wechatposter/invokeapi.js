/**
 * Created by Json on 2015/1/7.
 */
(function () {
	var templateHandler = window.templateHandler || '/Template/GetLayouts?count=50';
	var workeditorHandler = window.workeditorHandler || '/work/WorkEditor';
	var workslist = window.workslist || '/work/UserWorks';
	var addPageHandler = window.addPageHandler || '/work/AddPage';
	var delPageHandler = window.delPageHandler || '/work/DelPage';
	var edtTxtHandler = window.edtTxtHandler || '/Media/TextBatch'; //　编辑文字
	var uploadHandler = window.uploadHandler || '/Media/WebEditorUploador'; // 编辑图片
	var worksetHandler = window.worksetHandler || '/work/WorkSetting';
	var musicCatHandler = window.musicCatHandler || '/Media/MusicCategory';
	var musicsHandler = window.musicsHandler || '/Media/Musics';
	var fileUploadHandler = window.fileUploadHandler || '/Media/Upload';
	var musicHandler = window.musicHandler || '/Media/CreateMusic';
	var uptPageHandler = window.uptPageHandler || '/work/SortPage';
	var useSetting = window.useSetting || '/User/UserSetting';
	var GetUserSetting = window.GetUseSetting || '/User/GetUserSetting';
	var removeImageFromMultiImg = window.removeImageFromMultiImg || '/Media/RemoteItemFromMedias';
	var sortMultiImage = window.sortMultiImage || "/Media/SortRichMedia";
	var modifyPasswd = window.modifyPasswd || "/Account/ModifyPwd";
	var getUserInfo = window.getUserInfo || "/Account/GetUserInfo";

	// 获取所有版式，添加新页面选择的时候会用到
	var getAllTemplate = function ( callback ) {
		$.post( templateHandler, callback );
	};

	// 得到用户具体配置了哪些页面的数据
	var getPageData = function ( callback ) {
		$.post( workeditorHandler + "/" + window.workId, callback );
	};

	var getUserSetting = function ( callback ) {
		$.post( GetUserSetting, callback );
	};

	var saveUserSetting = function ( data, callback ) {
		var arg = {
			url : useSetting,
			success : callback,
			data : data
		};
		uploadData( arg );
	};

	var addPage = function ( templateid, callback ) {
		$.post( addPageHandler, {"workId" : window.workId, "layoutId" : templateid}, callback );
	};

	var deletePage = function ( guid, callback ) {
		$.post( delPageHandler, {"workId" : window.workId, "guid" : guid}, callback );
	};

	var getMusicCategory = function ( callback ) {
		$.post( musicCatHandler, callback );
	};

	// 得到某个具体分类的音乐
	var getMusicListByCategory = function ( cat, callback ) {
		$.post( musicsHandler, {"cat" : cat}, callback );
	};


	// 上传数据的都要调用这个接口
	var uploadData = function ( arg ) {
		var xhr = new XMLHttpRequest();
		arg.onerror && xhr.addEventListener( "error", arg.onerror, false );
		arg.progress && xhr.upload.addEventListener( "progress", arg.progress, false );
		xhr.addEventListener( "readystatechange", function () {
			if ( xhr.readyState == 4 ) {
				try {
					arg.success && arg.success( JSON.parse( xhr.responseText ) );
				}
				catch ( e ) {
				}
			}
		} );
		xhr.open( "post", arg.url );
		xhr.send( arg.data );
		return xhr; // 有些上传需要取消操作，需要能夠取到这个xhr
	};

	var uploadUserMusic = function ( arg ) {
		arg.url = fileUploadHandler;
		return uploadData( arg );
	};

	var syncUploadMusic = function ( data, callback ) {
		// 每次调用上传音乐接口，都会把音乐传到一个第三方服务器上，调用成功之后需要在调用这个接口和我们的服务器同步数据
	    $.post(musicHandler, { "Name": data.FileName, "Url": data.Url, "Category": "自定义", "MusicType": 2 }, function (rdata) {
			callback && callback( rdata );
		} );
	};

	// 保存用户设置，由于以前是一个页面设置所有信息，而现在将基本设置与音乐设置的ui分开了，所以在设置音乐的时候需要再调用这个接口
	var saveConfig = function ( arg ) {
		arg.url = worksetHandler;
		return uploadData( arg );
	};

	var saveTextConfig = function ( data, callback ) {
		$.post( edtTxtHandler, data, callback );
	};

	var saveImageConfig = function ( arg ) {
		arg.url = uploadHandler;
		uploadData( arg );
	};

	// 从多图中删除一幅图的接口
	var deleteImageFromMultiImages = function ( data, callback ) {
		$.post( removeImageFromMultiImg, {pid : data.pid, mid : data.mid}, callback )
	};

	// 多图排序
	var toSortMultiImage = function ( data, callback ) {
		$.post( sortMultiImage, data, callback );
	};

	// 修改云起账号的密码
	var changePassword = function ( password, callback ) {
		$.post( modifyPasswd, {
			password : password
		}, callback );
	};

	var getTheUserInfo = function ( callback ) {
		$.post( getUserInfo, null, callback );
	};

	window.fpInvokeAPI = {
		getAllTemplate : getAllTemplate,
		//getPageData : getPageData,
		addPage : addPage,
		deletePage : deletePage,
		getMusicCategory : getMusicCategory,
		getMusicListByCategory : getMusicListByCategory,
		uploadUserMusic : uploadUserMusic,
		syncUploadMusic : syncUploadMusic,
		uploadData : uploadData,
		saveConfig : saveConfig,
		saveTextConfig : saveTextConfig,
		saveImageConfig : saveImageConfig,
		getUserSetting : getUserSetting,
		saveUserSetting : saveUserSetting,
		deleteImageFromMultiImages : deleteImageFromMultiImages,
		toSortMultiImage : toSortMultiImage,
		changePassword : changePassword,
		getUserInfo : getTheUserInfo
	}
})();