namespace csharptov;

public abstract class Entity
{
	public readonly string name;
	
	protected readonly uint defence;
	protected readonly float power;

	protected Entity(string name, uint defence, float power)
	{
		this.name = name;
		this.defence = defence;
		this.power = power;
	}

	public abstract float GetAttackPower();
	public abstract float GetDefencePower();
}

public abstract class Human : Entity
{
	public uint XP { get; protected set; }
	public uint WeaponQuality { get; private set; }

	protected Human(string name, uint defence, float power, uint xp, uint weaponQuality) : base(name, defence, power)
	{
		XP = xp;
		WeaponQuality = weaponQuality;
	}
	
	public override sealed float GetAttackPower()
	{
		return power * XP * WeaponQuality;
	}
	
	public override sealed float GetDefencePower()
	{
		return defence * XP * WeaponQuality;
	}
}

public class Player : Human
{
	public uint HP { get; private set; } = 100;
	
	public Player(string name, uint defence, float power, uint xp, uint weaponQuality) : base(name, defence, power, xp, weaponQuality) { }

	public void IncrementXP()
	{
		XP += 1;
	}
	
	public void DecrementHP()
	{
		HP -= 1;
	}
}

public class Wolf : Entity
{
	private readonly uint agile;

	public Wolf (string name, uint defence, float power, uint agile) : base(name, defence, power)
	{
		this.agile = agile;
	}

	public override float GetAttackPower()
	{
		return power * agile;
	}

	public override float GetDefencePower()
	{
		return defence * agile;
	}
}

public class Guard : Human
{
	public Guard (string name, uint defence, float power, uint xp, uint weaponQuality) : base(name, defence, power, xp, weaponQuality) { }
}

public static class Program
{
	private const string HIT_COMMAND = "hit";
	private const string DEFENCE_COMMAND = "defence";
	private const string EXIT_COMMAND = "exit";
	
	private const string DEFAULT_PLAYER_NAME = "Player";
	private const uint START_XP = 1;
	private const uint MIN_START_WEAPON_QUALITY = 2;
	private const uint MAX_START_WEAPON_QUALITY = 5;
	private const uint MIN_START_DEFENCE = 2;
	private const uint MAX_START_DEFENCE = 5;
	
	private const uint MAX_DEFAULT_ENEMY_DEFENCE = 5;
	private const uint MAX_DEFAULT_AGILE = 3;
	private const uint MAX_DEFAULT_ENEMY_WEAPON_QUALITY = 3;
	private const uint MAX_DEFAULT_ENEMY_XP = 3;

	private static Player playerInstance = null!;

	public static void Main()
	{
		PrintGameStartMessages();
		CreatePlayerCharacter();
		StartMainGameLoop();
	}

	private static void PrintGameStartMessages()
	{
		Console.WriteLine("Tiny RPG");
		Console.WriteLine($"Print \"{HIT_COMMAND}\" to attack an enemy");
		Console.WriteLine($"Print \"{DEFENCE_COMMAND}\" to defence enemy's attack and counterattack");
		Console.WriteLine($"Print \"{EXIT_COMMAND}\" to finish the game");
		Console.WriteLine();
	}

	private static void CreatePlayerCharacter()
	{
		Console.WriteLine("Name of your character:");
		playerInstance = new Player(GetPlayerName(), GetPlayerStartDefence(), GetStartPower(), START_XP, GetPlayerStartWeaponQuality());
		PrintPlayerStatsWithEmptyLine();
	}

	private static string GetPlayerName()
	{
		return Console.ReadLine() ?? DEFAULT_PLAYER_NAME;
	}

	private static uint GetPlayerStartDefence()
	{
		return (uint) Random.Shared.Next((int) MIN_START_DEFENCE, (int) MAX_START_DEFENCE);
	}

	private static float GetStartPower()
	{
		return Random.Shared.NextSingle() + 1.0f;
	}

	private static uint GetPlayerStartWeaponQuality()
	{
		return (uint) Random.Shared.Next((int) MIN_START_WEAPON_QUALITY, (int) MAX_START_WEAPON_QUALITY);
	}

	private static void PrintPlayerStatsWithEmptyLine()
	{
		Console.WriteLine($"Your character - {playerInstance.name}, attack power - {playerInstance.GetAttackPower()}, defence power - {playerInstance.GetDefencePower()}, XP - {playerInstance.XP}, weapon quality - {playerInstance.WeaponQuality}");
		Console.WriteLine();
	}

	private static void StartMainGameLoop()
	{
		while (true)
		{
			Entity currentEnemy = CreateNewEnemy();
			PrintEnemyStats(currentEnemy);

			Console.WriteLine("What will you do?");
			string? userInput = Console.ReadLine();

			if (userInput is null or EXIT_COMMAND)
			{
				break;
			}

			switch (userInput)
			{
				case HIT_COMMAND:
					TryKillEnemy(currentEnemy);
					break;
				case DEFENCE_COMMAND:
					TryDefenceEnemyAttack(currentEnemy);
					break;
				default:
					continue;
			}

			if (playerInstance.HP == 0)
			{
				Console.WriteLine($"You are dead. Your XP is - {playerInstance.XP}");
				Console.WriteLine();
				Console.WriteLine("Create new character");
				Console.WriteLine();
				CreatePlayerCharacter();
			}
			else
			{
				PrintPlayerStatsWithEmptyLine();
			}
		}
	}

	private static Entity CreateNewEnemy()
	{
		int randomNumber = Random.Shared.Next(2);
		return randomNumber == 0 ? CreateNewWolf() : CreateNewGuard();
	}

	private static Wolf CreateNewWolf()
	{
		return new Wolf("Wolf", GetRandomDefence(), GetStartPower(), GetRandomAgile());
	}

	private static uint GetRandomDefence()
	{
		return (uint) Random.Shared.Next((int) (MAX_DEFAULT_ENEMY_DEFENCE * playerInstance.XP));
	}

	private static uint GetRandomAgile()
	{
		return (uint) Random.Shared.Next((int) (MAX_DEFAULT_AGILE * playerInstance.XP));
	}

	private static Guard CreateNewGuard()
	{
		return new Guard("Guard", GetRandomDefence(), GetStartPower(), GetRandomXP(), GetRandomEnemyWeaponQuality());
	}

	private static uint GetRandomEnemyWeaponQuality()
	{
		return (uint) Random.Shared.Next((int) (MAX_DEFAULT_ENEMY_WEAPON_QUALITY * playerInstance.XP));
	}

	private static uint GetRandomXP()
	{
		return (uint) Random.Shared.Next((int) (MAX_DEFAULT_ENEMY_XP * playerInstance.XP));
	}
	
	private static void PrintEnemyStats (Entity currentEnemy)
	{
		Console.WriteLine($"Your next enemy - {currentEnemy.name}, attack = {currentEnemy.GetAttackPower()}, defence - {currentEnemy.GetDefencePower()}");
		Console.WriteLine();
	}
	
	private static void TryKillEnemy(Entity? currentEnemy)
	{
		bool canKillEnemy = currentEnemy?.GetDefencePower() <= playerInstance.GetAttackPower();

		if (canKillEnemy == true)
		{
			Console.WriteLine("Enemy was killed!");
			IncrementPlayerXPWithMessage();
		}
		else
		{
			Console.WriteLine($"Your attack is too low ({playerInstance.GetAttackPower()})");
			DecrementPlayerHPWithMessage();
		}
	}

	private static void TryDefenceEnemyAttack(Entity? currentEnemy)
	{
		bool canDefence = playerInstance.GetDefencePower() >= currentEnemy?.GetAttackPower();

		if (canDefence == true)
		{
			Console.WriteLine("Enemy was killed by counterattack!");
			IncrementPlayerXPWithMessage();
		}
		else
		{
			Console.WriteLine($"Your defence is too low ({playerInstance.GetDefencePower()})");
			DecrementPlayerHPWithMessage();
		}
	}

	private static void IncrementPlayerXPWithMessage()
	{
		playerInstance.IncrementXP();
		PrintPlayerGainXPWithEmptyLine();
	}

	private static void PrintPlayerGainXPWithEmptyLine()
	{
		Console.WriteLine($"You gain 1 XP, your actual XP is {playerInstance.XP}");
		Console.WriteLine();
	}

	private static void DecrementPlayerHPWithMessage()
	{
		playerInstance.DecrementHP();
		PrintPlayerLostHPWithEmptyLine();
	}

	private static void PrintPlayerLostHPWithEmptyLine()
	{
		Console.WriteLine($"You lost 1 HP, your actual HP is {playerInstance.HP}");
		Console.WriteLine();
	}
}