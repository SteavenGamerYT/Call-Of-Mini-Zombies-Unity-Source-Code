public enum PROTOCOLS
{
	VERSION = 1,
	CG_HEARTBEAT = 1048576,
	GC_HEARTBEAT = 1052672,
	CG_ROOMLIST = 2,
	GC_ROOMLIST = 4098,
	CG_CREATEROOM = 3,
	GC_CREATEROOM = 4099,
	CG_DESTROYROOM = 4,
	GC_DESTROYROOM = 4100,
	GC_DESTROYROOM_NOTIFY = 4356,
	CG_JOINROOM = 5,
	GC_JOINROOM = 4101,
	GC_JOINROOM_NOTIFY = 4357,
	CG_LEAVEROOM = 6,
	GC_LEAVEROOM = 4102,
	GC_LEAVEROOM_NOTIFY = 4358,
	CG_KICKUSER = 7,
	GC_KICKUSER = 4103,
	GC_KICKUSER_NOTIFY = 4359,
	GC_BYKICKED_NOTIFY = 4615,
	CG_STARTGAME = 8,
	GC_STARTGAME = 4104,
	GC_STARTGAME_NOTIFY = 4360,
	CG_USERBIRTH = 9,
	GC_USERRBIRTH_NOTIFY = 4361,
	CG_ENEMY_DEAD = 10,
	GC_ENEMY_DEAD = 4106,
	GC_ENEMY_DEAD_NOTIFY = 4362,
	GC_MASTER_CHANGE = 4363,
	CG_USER_DO_REBIRTH = 12,
	GC_USER_DO_REBIRTH = 4108,
	GC_USER_DO_REBIRTH_NOTIFY = 4364,
	CG_ON_USER_DEAD = 13,
	CG_MAP_STATE = 14,
	GC_MAP_STATE = 4110,
	CG_USERCFG = 65537,
	GC_USERCFG_NOTIFY = 69889,
	CG_USERSTATUS = 65538,
	GC_USERSTATUS_NOTIFY = 69890,
	CG_USERACTION = 65539,
	GC_USERACTION_NOTIFY = 69891,
	CG_USERCHANGEWEAPON = 65540,
	GC_USERCHANGEWEAPON_NOTIFY = 69892,
	CG_USER_SNIPER_FIRE = 65541,
	GC_USER_SNIPER_FIRE_NOTIFY = 69893,
	CG_ENEMY_BIRTH = 65542,
	GC_ENEMY_BIRTH_NOTIFY = 69894,
	CG_ENEMYSTATUS = 65543,
	GC_ENEMYSTATUS_NOTIFY = 69895,
	CG_ENEMY_GOTHIT = 65544,
	GC_ENEMY_GOTHIT_NOTIFY = 69896,
	CG_ENEMY_LOOT = 65545,
	GC_ENEMY_LOOT_NOTIFY = 69897,
	CG_USER_INJURY = 65546,
	GC_USER_INJURY_NOTIFY = 69898,
	CG_ENEMY_CHANGE_TARGET = 65547,
	GC_ENEMY_CHANGE_TARGET_NOTIFY = 69899,
	CG_USER_REBIRTH = 65548,
	GC_USER_REBIRTH_NOTIFY = 69900,
	CG_ENEMY_REMOVE = 65549,
	GC_ENEMY_REMOVE_NOTIFY = 69901,
	CG_USER_REPORT_DATA = 65550,
	GC_USER_REPORT_DATA_NOTIFY = 69902,
	CG_GAME_OVER = 65551,
	GC_GAME_OVER_NOTIFY = 69903
}
