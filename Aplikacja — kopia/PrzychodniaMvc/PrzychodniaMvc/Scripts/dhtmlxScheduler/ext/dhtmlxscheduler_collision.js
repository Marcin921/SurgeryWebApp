/*
@license
dhtmlxScheduler.Net v.3.3.12 

This software is covered by DHTMLX Evaluation License. Contact sales@dhtmlx.com to get Commercial or Enterprise license. Usage without proper license is prohibited.

(c) Dinamenta, UAB.
*/
Scheduler.plugin(function(e){!function(){function t(t){var s=e._get_section_view();s&&t&&(i=e.getEvent(t)[e._get_section_property()])}var i,s;e.config.collision_limit=1,e.attachEvent("onBeforeDrag",function(e){return t(e),!0}),e.attachEvent("onBeforeLightbox",function(i){var a=e.getEvent(i);return s=[a.start_date,a.end_date],t(i),!0}),e.attachEvent("onEventChanged",function(t){if(!t||!e.getEvent(t))return!0;var i=e.getEvent(t);if(!e.checkCollision(i)){if(!s)return!1;i.start_date=s[0],i.end_date=s[1],
i._timed=this.isOneDayEvent(i)}return!0}),e.attachEvent("onBeforeEventChanged",function(t,i,s){return e.checkCollision(t)}),e.attachEvent("onEventAdded",function(t,i){var s=e.checkCollision(i);s||e.deleteEvent(t)}),e.attachEvent("onEventSave",function(t,i,s){if(i=e._lame_clone(i),i.id=t,!i.start_date||!i.end_date){var a=e.getEvent(t);i.start_date=new Date(a.start_date),i.end_date=new Date(a.end_date)}return i.rec_type&&e._roll_back_dates(i),e.checkCollision(i)}),e._check_sections_collision=function(t,i){
var s=e._get_section_property();return t[s]==i[s]&&t.id!=i.id?!0:!1},e.checkCollision=function(t){var s=[],a=e.config.collision_limit;if(t.rec_type)for(var n=e.getRecDates(t),r=0;r<n.length;r++)for(var d=e.getEvents(n[r].start_date,n[r].end_date),o=0;o<d.length;o++)(d[o].event_pid||d[o].id)!=t.id&&s.push(d[o]);else{s=e.getEvents(t.start_date,t.end_date);for(var l=0;l<s.length;l++)if(s[l].id==t.id){s.splice(l,1);break}}var _=e._get_section_view(),h=e._get_section_property(),c=!0;if(_){for(var u=0,l=0;l<s.length;l++)s[l].id!=t.id&&this._check_sections_collision(s[l],t)&&u++;

u>=a&&(c=!1)}else s.length>=a&&(c=!1);if(!c){var g=!e.callEvent("onEventCollision",[t,s]);return g||(t[h]=i||t[h]),g}return c}}()});