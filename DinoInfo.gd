extends Node

signal dino_deployed

var dino_id := 0
var dino_list := [
	preload("res://actors/dinos/MegaDino.tscn"),
	preload("res://actors/dinos/TankyDino.tscn"),
	preload("res://actors/dinos/WarriorDino.tscn"),
]
var dinos_deploying := []
var timer_list := []

# {tanky: [health, dmg], mega: [health]}
var upgrades_list := {"warrior": ["fire"], "tanky": ["ice"]}


func has_upgrade(dino: String, upgrade: String) -> bool:
	return dino in upgrades_list and upgrade in upgrades_list[dino]

	print(upgrades_list)


func add_upgrade(dino: String, upgrade: String) -> void:
	if dino in upgrades_list:
		upgrades_list[dino].append(upgrade)
	else:
		upgrades_list[dino] = [upgrade]


func _on_dino_deployed():
	# add to list of dinos just deployed
	# prevents this type of dino from being deployed
	dinos_deploying.append(dino_id)

	# wait for the dino-specific delay
	var delay = get_dino_timer_delay()
	var dinos_deploying_timer = Timer.new()
	dinos_deploying_timer.one_shot = true
	add_child(dinos_deploying_timer)

	dinos_deploying_timer.connect("timeout", self, "_on_dinos_deploying_timer_timeout", [dino_id])
	dinos_deploying_timer.start(delay)

	# re-emit signal
	# allows for easier connection since it's a singleton
	emit_signal("dino_deployed")


func _on_dinos_deploying_timer_timeout(id):
	# once the delay is over, let the dino be deployed again
	dinos_deploying.remove(dinos_deploying.find(id))

# find the delay based on the dino
func get_dino_timer_delay():
	return get_dino_property("deploy_delay")

func get_dino_property(prop: String):
	# instance it, get the variable we want, then remove it
	var dino_scene = dino_list[dino_id]
	var dino_instance = dino_scene.instance()

	var property = dino_instance.get(prop)

	dino_instance.queue_free()
	return property

func reset_deploys():
	dinos_deploying = []
	timer_list = []
