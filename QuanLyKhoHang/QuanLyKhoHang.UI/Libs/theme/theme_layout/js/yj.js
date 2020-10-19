var btn_lod = true;
var yj_hide_lod = true;
var v2_api_url = 'https://uplibra.io/json-api/en/';
// 登录
function facebook_login(code) {
	swal.fire({
		title: "Login",
		text: "You are logging in your UPLibra account.",
		allowOutsideClick: false,
		allowEscapeKey: false,
		onOpen: function () {
			swal.showLoading()
		}
	})
	$.ajax({
		type: "GET",
		url: ajaxUrl + '/module/ajax/' + _yj_lang + '/facebook-login.php',
		data: { code: code },
		dataType: 'json',
		success: function (data) {
			swal.close();
			if (data.code == 200) {
				swal.fire({
					text: data.msg,
					type: "success",
					allowOutsideClick: false,
					allowEscapeKey: false,
					timer: 1000,
					showConfirmButton: false
				})
				setTimeout(function () {
					window.location.href = "https://uplibra.io/dashboard";
				}, 2000);
			} else {
				swal.fire({
					text: data.msg,
					type: "error",
					timer: 2000,
					allowOutsideClick: false,
					allowEscapeKey: false,
					showConfirmButton: false
				});
				setTimeout(function () {
					window.location.href = "https://uplibra.io/";
				}, 2000);
			}
		}
	});
}
// 获取验证码
function yj_get_code(obj) {
	var i = 1;
	var email = $("#yj_user_chek_email").val();
	if (email == '') {
		swal.fire({
			text: "Please enter your e-mail address.",
			type: "error",
			timer: 1000,
			showConfirmButton: false
		});
	} else {
		if (btn_lod) {
			btn_lod = false;
			var text = $(obj).html();
			$(obj).addClass('kt-spinner kt-spinner--sm kt-spinner--danger disabled');
			$.ajax({
				type: "POST",
				url: ajaxUrl + '/module/ajax/' + _yj_lang + '/yj-mail-chek-link.php',
				data: { email: email },
				dataType: 'json',
				success: function (data) {
					if (data.code == 200) {
						swal.fire({
							text: data.msg,
							type: "success",
							timer: 2000,
							showConfirmButton: false,
							allowOutsideClick: false,
							allowEscapeKey: false
						});
						var timeStop = setInterval(function () {
							if (i == 120) {
								$(obj).html(text);
								$(obj).removeClass('disabled');
								clearInterval(timeStop);
								btn_lod = true;
							} else {
								printnr = 120 - i;
								$(obj).html(printnr + " s");
								i++;
							};
						}, 1000);
					} else {
						swal.fire({
							text: data.msg,
							type: "error",
							timer: 1000,
							showConfirmButton: false,
							allowOutsideClick: false,
							allowEscapeKey: false
						});
						$(obj).removeClass('disabled');
						btn_lod = true;
					}
				},
				complete: function () {
					$(obj).removeClass('kt-spinner kt-spinner--sm kt-spinner--danger');
				}
			});
		}
	}
}
//邮箱验证
function yj_chek_email(key, email) {
	swal.fire({
		title: "Verify E-mail",
		text: "E-mail Verifying",
		onOpen: function () {
			swal.showLoading()
		}
	})
	$.ajax({
		type: "POST",
		url: ajaxUrl + '/module/ajax/' + _yj_lang + '/chek-email-v2.php',
		data: { key: key, email: email },
		dataType: 'json',
		success: function (data) {
			if (data.code = 200) {
				swal.fire({
					text: data.msg,
					type: "success",
					timer: 2000,
					showConfirmButton: false,
				});
				setTimeout(function () {
					window.location.href = "https://uplibra.io/dashboard";
				}, 2000);
			} else {
				swal.fire({
					text: data.msg,
					type: "error",
					timer: 1000,
					showConfirmButton: false,
				});
			}
		}
	});
}
//抽奖
function yj_lottery(token) {
	if (btn_lod) {
		btn_lod = false;
		swal.fire({
			title: "Win Libra",
			text: "Please wait...",
			allowOutsideClick: false,
			allowEscapeKey: false,
			onOpen: function () {
				swal.showLoading()
			}
		})
		$.ajax({
			type: "POST",
			data: {
				token: token
			},
			url: ajaxUrl + '/module/ajax/' + _yj_lang + '/user-lottery.php',
			dataType: 'json',
			success: function (data) {
				swal.close();
				btn_lod = true;
				if (data.code == 200) {
					swal.fire({
						text: data.msg,
						timer: 1000,
						showConfirmButton: false,
						type: "success"
					});
					setTimeout(function () {
						window.location.reload();
					}, 2000);
				} else {
					swal.fire({
						text: data.msg,
						timer: 1000,
						showConfirmButton: false,
						type: "warning"
					})
				}
			}
		});
	}
}
//领奖
function yj_lottery_b(obj) {
	if (btn_lod) {
		btn_lod = false;
		swal.fire({
			title: "Loyal Bonus",
			text: "Please wait...",
			allowOutsideClick: false,
			allowEscapeKey: false,
			onOpen: function () {
				swal.showLoading()
			}
		})
		$.ajax({
			type: "POST",
			url: ajaxUrl + '/module/ajax/' + _yj_lang + '/user-lottery-b.php',
			dataType: 'json',
			success: function (data) {
				swal.close();
				btn_lod = true;
				if (data.code == 200) {
					swal.fire({
						text: data.msg,
						timer: 1000,
						showConfirmButton: false,
						type: "success"
					});
					setTimeout(function () {
						window.location.reload();
					}, 2000);
				} else {
					swal.fire({
						text: data.msg,
						timer: 1000,
						showConfirmButton: false,
						type: "warning"
					})
				}
			}
		});
	}
}
// 倒计时
function yj_lottery_countdown(intDiff) {
	var intDiff = parseInt(intDiff);
	var time = setInterval(function () {
		var day = 0,
			hour = 0,
			minute = 0,
			second = 0;
		if (intDiff <= 0) {
			setTimeout(function () {
				window.location.reload();
			}, 2000);
			return;
		}
		if (intDiff > 0) {
			day = Math.floor(intDiff / (60 * 60 * 24));
			hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
			minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
			second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
		}
		if (minute <= 9) minute = '0' + minute;
		if (second <= 9) second = '0' + second;
		$('#yj_lottery_countdown .h').html(hour);
		$('#yj_lottery_countdown .m').html(minute);
		$('#yj_lottery_countdown .s').html(second);
		intDiff--;
	}, 1000);
}
// faq切换
function yj_faq_tog(self, obj) {
	$(self).addClass('kt-nav__item--active').siblings().removeClass('kt-nav__item--active');
	$(obj).show().siblings().hide();
}

$(function () {
	var wow = new WOW({
		boxClass: 'wow',
		animateClass: 'animated',
		offset: 0,
		mobile: true,
		live: true,
		callback: function (box) {
		},
		scrollContainer: null,
		resetAnimation: true,
	});
	wow.init();
})
function sliver_payout_more(obj) {
	var page = $(obj).attr("data-page");
	$(obj).addClass('kt-spinner kt-spinner--right kt-spinner--sm');
	queuePaymentTable(page);
}
function sliver_stop_login() {
	swal.fire({
		text: "Suspend new registration and log in, please try it later.",
		timer: 3000,
		showConfirmButton: false,
		type: "warning"
	})
}

function yj_Reset_pwd(obj) {
	var pwd = $("#yj_find_pwd").val();
	pwd = pwd.replace(/\s+/g, "");
	var pwd_comfirm = $("#yj_find_pwd_comfirm").val();
	pwd_comfirm = pwd_comfirm.replace(/\s+/g, "");
	if (pwd.length < 6 || pwd.length > 20) {
		swal.fire({
			text: "The password should be  6 - 20 characters.",
			type: "error",
			timer: 2500,
			showConfirmButton: false,
		});
		return;
	}
	if (pwd_comfirm != pwd) {
		swal.fire({
			text: "The password confirmation does not match.",
			type: "error",
			timer: 2500,
			showConfirmButton: false,
		});
		return;
	}
	$(obj).addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light disabled');
	if (btn_lod) {
		btn_lod = false;
		$.ajax({
			url: v2_api_url + 'home/get/reset_password',
			type: 'POST',
			dataType: 'json',
			data: {
				pwd: pwd,
				pwd_comfirm: pwd_comfirm
			},
		}).done(function (apiRes) {
			var code = apiRes.code;
			var msg = apiRes.msg;
			if (code == 200) {
				swal.fire({
					text: msg,
					type: "success",
					timer: 2500,
					showConfirmButton: false,
				});
				setTimeout(function () {
					window.location.href = "https://uplibra.io/signin";
				}, 2000);
			} else {
				swal.fire({
					text: msg,
					type: "error",
					timer: 2500,
					showConfirmButton: false,
				});
				return;
			}
		}).fail(function () {
			swal.fire({
				text: msg,
				type: "error",
				timer: 2500,
				showConfirmButton: false,
			});
			return;
		}).always(function () {
			btn_lod = true;
			$(obj).removeClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light disabled');
		});
	}

}
function set_user_name(obj) {
	var user_name = $("#yj_update_username").val();
	user_name = user_name.replace(/\s+/g, "");
	if (user_name.length < 2 || user_name.length > 15) {
		swal.fire({
			text: "The username should be  2 - 15 characters.",
			timer: 3000,
			showConfirmButton: false,
			type: "warning"
		})
		return;
	}
	$(obj).addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light disabled');
	if (btn_lod) {
		btn_lod = false;
		$.ajax({
			url: v2_api_url + 'user/up_user_name',
			type: 'POST',
			dataType: 'json',
			data: {
				user_name: user_name
			},
		}).done(function (apiRes) {
			var code = apiRes.code;
			var msg = apiRes.msg;
			if (code == 200) {
				swal.fire({
					text: msg,
					type: "success",
					timer: 2500,
					showConfirmButton: false,
				});
				setTimeout(function () {
					window.location.reload();
				}, 2000);
			} else {
				swal.fire({
					text: msg,
					timer: 3000,
					showConfirmButton: false,
					type: "warning"
				})
			}
		}).fail(function () {
			swal.fire({
				text: "Server error, please try later.",
				type: "error",
				timer: 2500,
				showConfirmButton: false,
			});
			return;
		}).always(function () {
			btn_lod = true;
			$(obj).removeClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light disabled');
		});
	}
}

function yj_user_Obtain() {
	var userObtain = localStorage.getItem("userObtain");
	if (userObtain) {
		return;
	}
	if (yj_hide_lod) {
		yj_hide_lod = false;
		$.ajax({
			url: v2_api_url + "user/user_obtain_claim",
			type: "POST",
			dataType: "json",
			success: function (apiData) {
				var code = apiData.code;
				if (code == 409) {
					yj_user_Obtain();
				} else if (code == 200) {
					localStorage.setItem("userObtain", "1");
				}
			},
			error: function () {
				yj_user_Obtain();
			},
			complete: function () {
				yj_hide_lod = true;
			}
		});
	}
}