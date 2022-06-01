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

	println('Your character - $player\n')
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
	for true {
		current_enemy := create_enemy(player)

		println('Current enemy - $current_enemy\n')

		user_input := os.input('What will you do?\n').trim_space()

		if user_input in ['', exit_command] {
			break
		}

		match user_input {
			hit_command {
				try_killing_enemy(mut player, current_enemy)
			}
			defence_command {
				try_defending(mut player, current_enemy)
			}
			else {
				println('Unknown command: $user_input')
				continue
			}
		}

		if player.hp == 0 {
			println('You are dead. Your XP is - $player.get_xp()\n')
			println('Create a new character\n')
			player = create_player()
		} else {
			println('Your character - $player\n')
		}
	}
}

fn create_enemy(player Player) Entity {
	return if rand.bernoulli(0.5) or { true } {
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
		agility: get_random_agility(player)
	}
}

fn get_random_defence(player Player) u32 {
	return player.xp * rand.u32_in_range(1, max_default_enemy_defence) or { 1 }
}

fn get_random_agility(player Player) u32 {
	return player.xp * rand.u32_in_range(1, max_default_agility) or { 1 }
}

fn create_new_guard(player Player) Entity {
	return Human{
		name: 'Guard'
		defence: get_random_defence(player)
		power: get_common_start_power()
		weapon_quality: get_random_weapon_quality(player)
		xp: get_random_xp(player)
	}
}

fn get_random_weapon_quality(player Player) u32 {
	return player.xp * rand.u32_in_range(1, max_default_enemy_weapon_quality) or { 1 }
}

fn get_random_xp(player Player) u32 {
	return player.xp * rand.u32_in_range(1, (max_default_enemy_xp - 1)) or { 1 }
}

fn try_killing_enemy(mut player Player, enemy Entity) {
	can_defeat_enemy := enemy.get_defence_power() <= player.get_attack_power()

	if can_defeat_enemy {
		println('Enemy was killed!')
		increment_xp_and_print_message(mut player)
	} else {
		println('Your attack is too low ($player.get_attack_power())')
		decrement_hp_and_print_message(mut player)
	}
}

fn try_defending(mut player Player, enemy Entity) {
	can_defend := player.get_defence_power() >= enemy.get_attack_power()

	if can_defend {
		println('Enemy was killed by counterattack!')
		increment_xp_and_print_message(mut player)
	} else {
		println('Your defence is too low ($player.get_defence_power())')
		decrement_hp_and_print_message(mut player)
	}
}

fn increment_xp_and_print_message(mut player Player) {
	player.increment_xp()
	println('You gain 1 XP, your actual XP is $player.xp')
}

fn decrement_hp_and_print_message(mut player Player) {
	player.decrement_hp()
	println('You lose 1 HP, your actual HP is $player.hp')
}
