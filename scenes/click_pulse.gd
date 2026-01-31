extends MeshInstance3D

@export var fade_time: float = 0.5
var timer: float = 0.0

func _ready():
	if material_override == null:
		material_override = get_active_material(0).duplicate()

func _process(delta: float) -> void:
	timer += delta
	var a = 1.0 - (timer / fade_time)

	material_override.set_shader_parameter("alpha", clamp(a, 0.0, 1.0))

	if a <= 0.0:
		queue_free()
