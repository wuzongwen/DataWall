var FZFVIDEO='<div id="fz-videoBox">'+
'<div class="fz-loading">'+
'<div id="bfBtn" style="visibility:hidden;opacity:0;">'+
'<div class="spinner">'+
'<div class="rect1"></div>'+
'<div class="rect2"></div>'+
'<div class="rect3"></div>'+
'<div class="rect4"></div>'+
'<div class="rect5"></div>'+
'</div>'+
'<div id="sptxt">'+
'msg...'+
'</div>'+
'</div>'+
'</div>'+
'<video src="" id="fz-videoAct"></video>'+
'<div id="videoState">'+
'<div id="videoStateBox">'+
'<div id="playOrStop">'+
'<i class="iconfont layui-icon" id="playIcon">播放</i>'+
'</div>'+
'<div id="currentTimeBox">'+
'<span id="currentTimers">0:00</span>'+
'<span>/</span>'+
'<span id="allTimers">0:00</span>'+
'</div>'+
'<div id="percentage">'+
'<div id="currentPerc">'+
'</div>'+
'<div id="currentPercentage">'+
'</div>'+
'<div id="currentAll">'+
'</div>'+
'<div id="currentBG">'+
'</div>'+
'</div>'+
'<div id="videoSpeed">'+
'<div class="videoBoxRe">'+
'<i class="iconfont">&#xe603;</i>'+
'<ul id="videoSpeedList">'+
'<li class="videoSpeedStup" id="bs4">2.5倍</li>'+
'<li class="videoSpeedStup" id="bs3">2.0倍</li>'+
'<li class="videoSpeedStup" id="bs2">1.5倍</li>'+
'<li class="videoSpeedStup" id="bs1">默认</li>'+
'</ul>'+
'</div>'+
'</div>'+
'<div id="videoSound">'+
'<div id="videoSoundBox">'+
'<i class="iconfont" id="videoSoundStop">&#xe605;</i>'+
'<div id="soundBar">'+
'<div id="currentSound">'+
'</div>'+
'</div>'+
'</div>'+
'</div>'+
'</div>'+
'</div>'+
'</div>';function createVideo(dn,obj){var fzPools={}
var hcqStop=0;this.D=function(dms){var dom_=document.getElementById(dms);return dom_;}
this.setUrl=function(url){this.D('fz-videoAct').src=url;}
this.init=function(){this.setUrl(obj.url);if(obj.autoplay){this.videoDOM.autoplay=true;}else{this.videoDOM.autoplay=false;}
fzPools.VIDEO_DOM.onclick=function(){stopOrplay();}
fzPools.VIDEO_DOM.ontimeupdate=function(){if(fzPools.VIDEO_DOM.paused){fzPools.playIcon.innerHTML="&#xe602";}else{fzPools.playIcon.innerHTML="&#xe600";}
fzPools.work.style.width=videoCurrentTime()+"%";fzPools.work2.style.width=videoCurrentTime()+"%";fzPools.currentTm.innerHTML=timeConversion(parseInt(fzPools.VIDEO_DOM.currentTime));fzPools.allTime.innerHTML=timeConversion(parseInt(videoAllTime()));/*if(hcqStop==0){var hcq=parseInt((fzPools.VIDEO_DOM.buffered.end(0)/videoAllTime()).toFixed(2)*100);fzPools.currentBG.style.width=hcq+"%";}else{fzPools.currentBG.style.width=100+"%";}*/}
fzPools.VIDEO_DOM.onwaiting=function(){openLoading("on","加载中");}
fzPools.VIDEO_DOM.onplaying=function(){openLoading("off");}
fzPools.VIDEO_DOM.onvolumechange=function(){var soundSize=fzPools.VIDEO_DOM.volume;var soundBarWidth=getDomWidth('soundBar')*soundSize;fzPools.soundBar.style.width=soundBarWidth+"px";}
fzPools.percentage.onclick=function(){var widthCurr=mouseCurrentX('percentage')/getDomWidth('percentage');fzPools.VIDEO_DOM.currentTime=videoAllTime()*widthCurr;hcqStop=1;}
fzPools.playOrStop.onclick=function(){if(fzPools.VIDEO_DOM.paused){fzPools.VIDEO_DOM.play();fzPools.playIcon.innerHTML="&#xe600";}else{fzPools.VIDEO_DOM.pause();fzPools.playIcon.innerHTML="&#xe602";}}
fzPools.soundBarBox.onclick=function(){var soundBarWidth=getDomWidth('soundBar');var clickWidth=mouseCurrentX('soundBar');fzPools.VIDEO_DOM.volume=clickWidth/soundBarWidth.toFixed(2);}
fzPools.vSoundStop.onclick=function(){if(fzPools.VIDEO_DOM.muted){fzPools.VIDEO_DOM.muted=false;fzPools.VIDEO_DOM.volume=0.99;fzPools.vSoundStop.innerHTML='&#xe605;';}else{fzPools.VIDEO_DOM.muted=true;fzPools.VIDEO_DOM.volume=0;fzPools.vSoundStop.innerHTML='&#xe604;';}}
fzPools.bs1.onclick=function(){fzPools.VIDEO_DOM.playbackRate=1;}
fzPools.bs2.onclick=function(){fzPools.VIDEO_DOM.playbackRate=1.5;}
fzPools.bs3.onclick=function(){fzPools.VIDEO_DOM.playbackRate=2;}
fzPools.bs4.onclick=function(){fzPools.VIDEO_DOM.playbackRate=2.5;}}
this.createDom=function(){if(this.D("fz-videoBox")==null){this.D(dn).innerHTML=FZFVIDEO;this.videoDOM=this.D("fz-videoAct");saveDom();this.init();}else{console.log("播放器节点已存在");return;}}
this.overVideo=function(){var ovDom=document.getElementById(dn);var childs=ovDom.childNodes;var length=childs.length;for(var i=0;i<length;i++){ovDom.removeChild(childs[0]);}
FZ_VIDEO='';}
function saveDom(){fzPools.VIDEO_DOM=document.getElementById("fz-videoAct");fzPools.work=document.getElementById("currentPercentage");fzPools.work2=document.getElementById("currentPerc");fzPools.currentTm=document.getElementById("currentTimers");fzPools.allTime=document.getElementById("allTimers");fzPools.percentage=document.getElementById("percentage");fzPools.playOrStop=document.getElementById("playOrStop");fzPools.playIcon=document.getElementById("playIcon");fzPools.bfBtn=document.getElementById("bfBtn");fzPools.sptxt=document.getElementById("sptxt");fzPools.soundBarBox=document.getElementById("soundBar");fzPools.soundBar=document.getElementById("currentSound");fzPools.vSoundStop=document.getElementById("videoSoundStop");fzPools.currentBG=document.getElementById("currentBG");fzPools.bs1=document.getElementById("bs1");fzPools.bs2=document.getElementById("bs2");fzPools.bs3=document.getElementById("bs3");fzPools.bs4=document.getElementById("bs4");}
function getDomWidth(dmNm){var ddmm=document.getElementById(dmNm);return parseInt(ddmm.width||ddmm.offsetWidth||ddmm.clientWidth);}
function mouseCurrentX(dmNm,event){var e=event||window.event;var ddmm=document.getElementById(dmNm);var p_Left=parseInt(ddmm.getBoundingClientRect().left);return e.clientX-p_Left;}
function mouseCurrentY(dmNm,event){}
function stopOrplay(state){switch(state){case "play":fzPools.VIDEO_DOM.play();break;case "stop":fzPools.VIDEO_DOM.pause();break;default:if(fzPools.VIDEO_DOM.paused){fzPools.VIDEO_DOM.play();}else{fzPools.VIDEO_DOM.pause();}
break;}}
function videoCurrentTime(){return(fzPools.VIDEO_DOM.currentTime/videoAllTime()).toFixed(3)*100;}
function videoAllTime(){return fzPools.VIDEO_DOM.duration;}
function timeConversion(tim){var second=tim%60;var min=parseInt(tim/60);return(Array(2).join(0)+min).slice(-2)+':'+(Array(2).join(0)+second).slice(-2);}
function openLoading(states,msg){if(states=="on"){fzPools.bfBtn.style.visibility="visible";fzPools.bfBtn.style.opacity="1";fzPools.sptxt.innerHTML=msg;}else{fzPools.bfBtn.style.visibility="hidden";fzPools.bfBtn.style.opacity="0";fzPools.sptxt.innerHTML='';}}
this.createDom();}