extends MeshInstance3D

@export var fade_time: float = .2
var timer: float = 0.0

var mat : StandardMaterial3D

func _ready():
	mat = get_active_material(0) as StandardMaterial3D

func _process(delta):
	if timer < fade_time:
		timer += delta
		var a = 1.0 - timer / fade_time

		# use alpha scissor threshold
		mat.alpha_scissor_threshold = clamp(a, 0, 1)

		if a <= 0.01:
			queue_free()
