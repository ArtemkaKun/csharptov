module main

import os
import rand

const (
	hit_command              = 'hit'
	exit_command             = 'exit'
	defence_command          = 'defence'

	default_player_name      = 'Player'

	start_xp                 = 1
	min_start_weapon_quality = 2
	max_start_weapon_quality = 5

	min_start_defence        = 2
	max_start_defence        = 5
)

const (
	max_default_enemy_defence        = 5
	max_default_agility              = 3
	max_default_enemy_weapon_quality = 3
	max_default_enemy_xp             = 3
)

fn main() {
	print_game_start_message()
	mut player := create_player()
	start_main_game_loop(mut player)
}

fn print_game_start_message() {
	println('Tiny RPG')
	println('Print "$hit_command" to attack an enemy')
	println('Print "$defence_command" to defence enemy\'s attack and counterattack')
	println('Print "$exit_command" to finish the game\n')
}

fn create_player() Player {
	user_input := os.input('Name of your character: ').trim_space()
	name := if user_input == '' { default_player_name } else { user_input }

	player := Player{
		name: name
		xp: start_xp
		defence: get_player_start_defence()
		power: get_common_start_power()
		weapon_quality: get_player_start_weapon_quality()
	}

	println('Your character - ${Entity(player)}\n')
	return player
}

fn get_player_start_defence() u32 {
	return rand.u32_in_range(min_start_defence, max_start_defence) or { min_start_defence }
}

fn get_common_start_power() f32 {
	return rand.f32() + 1.0
}

fn get_player_start_weapon_quality() u32 {
	return rand.u32_in_range(min_start_weapon_quality, max_start_weapon_quality) or {
		min_start_weapon_quality
	}
}

fn start_main_game_loop(mut player Player) {
	// for true {
	current_enemy := create_enemy(player)

	println('Current enemy - $current_enemy\n')
	// }
}

fn create_enemy(player Player) Entity {
	// random_value := rand.intn(2) or { 0 }
	random_value := 1
	return if random_value == 0 {
		Entity(create_new_wolf(player))
	} else {
		Entity(create_new_guard(player))
	}
}

fn create_new_wolf(player Player) Entity {
	return Wolf{
		name: 'Wolf'
		defence: get_random_defence(player)
		power: get_common_start_power()
	}
}

fn get_random_defence(player Player) u32 {
	return rand.u32n(max_default_enemy_defence * player.xp) or { 0 }
}

fn get_random_agility(player Player) u32 {
	return rand.u32n(max_default_agility * player.xp) or { 0 }
}

fn create_new_guard(player Player) Entity {
	return Human{
		name: 'Guard'
		defence: get_random_defence(player)
		power: get_common_start_power()
		weapon_quality: get_random_weapon_quality(player)
	}
}

fn get_random_weapon_quality(player Player) u32 {
	return rand.u32n(max_default_enemy_weapon_quality * player.xp) or { 0 }
}
