module main

pub interface Entity {
	get_name() string
	get_attack_power() f32
	get_defence_power() f32
}

pub fn (entity Entity) str() string {
	return '$entity.get_name(), attack = $entity.get_attack_power(), defence - $entity.get_defence_power()'
}

pub struct BaseEntity {
	name string
mut:
	defence u32
	power   f32
}

pub fn (base BaseEntity) get_name() string {
	return base.name
}

pub struct Human {
	BaseEntity
mut:
	xp             u32
	weapon_quality u32
}

pub fn (human Human) get_attack_power() f32 {
	return human.power * human.xp * human.weapon_quality
}

pub fn (human Human) get_defence_power() f32 {
	return human.defence * human.xp * human.weapon_quality
}

pub fn (human Human) str() string {
	return Entity(human).str()
}

pub struct Player {
	Human
mut:
	hp u32 = 100
}

pub fn (player Player) str() string {
	return Entity(player).str()
}

pub struct Wolf {
	BaseEntity
	agility u32
}

pub fn (wolf Wolf) get_attack_power() f32 {
	return wolf.power * wolf.agility
}

pub fn (wolf Wolf) get_defence_power() f32 {
	return wolf.defence * wolf.agility
}

pub fn (wolf Wolf) str() string {
	return Entity(wolf).str()
}
