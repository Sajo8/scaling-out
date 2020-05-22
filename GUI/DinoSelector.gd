extends Node2D

var active_id := 0

var shot_ice = false
var shot_fire = false

var ice_disabled = true
var fire_disabled = true

func enable_ice():
	$Sprite4X.hide()
	ice_disabled = false

func enable_fire():
	$Sprite5X.hide()
	fire_disabled = false

func disable_ice():
	$Sprite4X.show()
	ice_disabled = true

func disable_fire():
	$Sprite5X.show()
	fire_disabled = true

func _ready() -> void:
	$Label1.text = "1"
	$Label2.text = "2"
	$Label3.text = "3"

	var lanes = get_tree().get_nodes_in_group("lanes")
	if lanes:
		for lane in lanes:
			lane.connect("dino_deployed", self, "_on_dino_deployed")

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("dino_1"):
		active_id = 0
		$Particles.position.x = 0
	elif event.is_action_pressed("dino_2"):
		active_id = 1
		$Particles.position.x = 140
	elif event.is_action_pressed("dino_3"):
		active_id = 2
		$Particles.position.x = 270
	# on ready remove X if need be
	# on button pressed:
	# just do the thing
	# then deactivate it
	elif event.is_action_pressed("dino_4"):
		if shot_ice or ice_disabled:
			return
		for m in get_all_dinos():
			if m.dino_name == "tanky":
				m.shoot_projectile()
				disable_ice()
				return
	elif event.is_action_pressed("dino_5"):
		if shot_fire or fire_disabled:
			return
		for m in get_all_dinos():
			if m.dino_name == "warrior":
				m.shoot_projectile()
				disable_fire()
				return
	else:
		pass
	DinoInfo.dino_id = active_id

func _on_dino_deployed():
	for m in get_all_dinos():
		if m.dino_name == "tanky":
			if not shot_ice and DinoInfo.has_upgrade("tanky", "ice"):
				enable_ice()
		if m.dino_name == "warrior":
			if not shot_fire and DinoInfo.has_upgrade("warrior", "fire"):
				enable_fire()

func get_all_dinos():
	var dinos = []
	var lanes = get_node("/root/CombatScreen/Lanes")
	for lane in lanes.get_children():
		var m = lane.get_children()
		if m:
			for dino in m:
				if "TankyDino" in dino.name or "WarriorDino" in dino.name:
					dinos.append(dino)
	return dinos


# get each deploy timer to be independent of the thing
# fix issues with non-mega dinos when dinos_deploying
#maybe look at shop and upgrades