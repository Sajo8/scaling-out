extends GeneralMonster

var monster_name = "mega"
var speed := Vector2(50, 0)
var health = 100
var dmg = 5

var variations = ["blue", "green", "orange"]

func _init():
	if DinoInfo.has_upgrade(monster_name, "speed"):
		speed.x += 10
	if DinoInfo.has_upgrade(monster_name, "health"):
		health += 25
	if DinoInfo.has_upgrade(monster_name, "dmg"):
		dmg += 3

	._init(speed, health, variations, dmg)
