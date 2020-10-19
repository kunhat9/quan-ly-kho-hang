// 横幅显示
var  uplibra_if_bannar = localStorage.getItem("uplibra_if_bannar");
if(!uplibra_if_bannar){
	$("#yj_top_bannar").show();
}else{
	var uplibra_close_bannar_date =  localStorage.getItem("uplibra_close_bannar_date");
	var days = yj_cha_date(uplibra_close_bannar_date);
	if(days>=3){
		$("#yj_top_bannar").show();
	}
}
//关闭bannar
function yj_close_bannar(obj) {
	var cr_date = new Date().getTime();
	localStorage.setItem("uplibra_if_bannar","true");
	localStorage.setItem("uplibra_close_bannar_date",cr_date);
	$("#yj_top_bannar").hide();
}

// js 计算时间差
function yj_cha_date(star_date){
	var end_date = new Date();
	var chha_date = end_date.getTime() - star_date;
	var days=Math.floor(chha_date/(24*3600*1000));
	return days;
}