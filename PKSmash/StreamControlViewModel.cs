using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PKSmash
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum Port
	{
		[Description("Port 1 (Red)")]
		Port1,

		[Description("Port 2 (Blue)")]
		Port2,

		[Description("Port 3 (Yellow)")]
		Port3,

		[Description("Port 4 (Green)")]
		Port4
	}

	public class StreamControlViewModel : BindableBase
	{
		private string player1Name = "nqztv";

		public string Player1Name
		{
			get { return this.player1Name; }
			set { SetProperty(ref this.player1Name, value); }
		}

		private string player2Name = "WWWWWWWWWWWWWWW";

		public string Player2Name
		{
			get { return this.player2Name; }
			set { SetProperty(ref this.player2Name, value); }
		}

		private int player1Score = 0;

		public int Player1Score
		{
			get { return this.player1Score; }
			set { SetProperty(ref this.player1Score, value); }
		}

		private int player2Score = 0;

		public int Player2Score
		{
			get { return this.player2Score; }
			set { SetProperty(ref this.player2Score, value); }
		}

		private Port player1Port = Port.Port1;

		public Port Player1Port
		{
			get
			{
				return this.player1Port;
			}
			set
			{
				if (Player2Port == value)
				{
					Port tempPort = Player1Port;
					SetProperty(ref this.player1Port, value);
					Player2Port = tempPort;
				}
				else
				{
					SetProperty(ref this.player1Port, value);
				}
			}
		}

		private Port player2Port = Port.Port2;

		public Port Player2Port
		{
			get
			{
				return this.player2Port;
			}
			set
			{
				if (Player1Port == value)
				{
					Port tempPort = Player2Port;
					SetProperty(ref this.player2Port, value);
					Player1Port = tempPort;
				}
				else
				{
					SetProperty(ref this.player2Port, value);
				}
			}
		}

		private string player1Character;

		public string Player1Character
		{
			get { return this.player1Character; }
			set { SetProperty(ref this.player1Character, value); }
		}

		private string player2Character;

		public string Player2Character
		{
			get { return this.player2Character; }
			set { SetProperty(ref this.player2Character, value); }
		}

		public StreamControlViewModel(IEventAggregator eventAggregator)
		{
			//this.eventAggregator = eventAggregator;
			//this.UpdateCommand = new DelegateCommand(this.OnUpdate, this.CanUpdate).ObservesProperty(() => Player1Name);
			this.ResetScoresCommand = new DelegateCommand(this.OnResetScores, this.CanResetScores).ObservesProperty(() => Player1Score).ObservesProperty(() => Player2Score);
			this.SwapPlayersCommand = new DelegateCommand(this.OnSwapPlayers, this.CanSwapPlayers);

			eventAggregator.GetEvent<WinEvent>().Subscribe(UpdateWinnerScore, ThreadOption.UIThread);
			eventAggregator.GetEvent<P1CharacterChangeEvent>().Subscribe(OnP1CharacterChange, ThreadOption.UIThread);
			eventAggregator.GetEvent<P2CharacterChangeEvent>().Subscribe(OnP2CharacterChange, ThreadOption.UIThread);
			eventAggregator.GetEvent<P3CharacterChangeEvent>().Subscribe(OnP3CharacterChange, ThreadOption.UIThread);
			eventAggregator.GetEvent<P4CharacterChangeEvent>().Subscribe(OnP4CharacterChange, ThreadOption.UIThread);
		}

		private void OnP1CharacterChange (string character)
		{
			if (Player1Port == Port.Port1)
			{
				Player1Character = character;
			}

			if (Player2Port == Port.Port1)
			{
				Player2Character = character;
			}
		}

		private void OnP2CharacterChange(string character)
		{
			if (Player1Port == Port.Port2)
			{
				Player1Character = character;
			}

			if (Player2Port == Port.Port2)
			{
				Player2Character = character;
			}
		}

		private void OnP3CharacterChange(string character)
		{
			if (Player1Port == Port.Port3)
			{
				Player1Character = character;
			}

			if (Player2Port == Port.Port3)
			{
				Player2Character = character;
			}
		}

		private void OnP4CharacterChange(string character)
		{
			if (Player1Port == Port.Port4)
			{
				Player1Character = character;
			}

			if (Player2Port == Port.Port4)
			{
				Player2Character = character;
			}
		}

		private void UpdateWinnerScore(string winner)
    {
			switch (winner)
			{
				case "00":
					if (Player1Port == Port.Port1)
					{
						Player1Score += 1;
					}
					if (Player2Port == Port.Port1)
					{
						Player2Score += 1;
					}
					break;
				case "01":
					if (Player1Port == Port.Port2)
					{
						Player1Score += 1;
					}
					if (Player2Port == Port.Port2)
					{
						Player2Score += 1;
					}
					break;
				case "02":
					if (Player1Port == Port.Port3)
					{
						Player1Score += 1;
					}
					if (Player2Port == Port.Port3)
					{
						Player2Score += 1;
					}
					break;
				case "03":
					if (Player1Port == Port.Port4)
					{
						Player1Score += 1;
					}
					if (Player2Port == Port.Port4)
					{
						Player2Score += 1;
					}
					break;
			}
		}

		//private IEventAggregator eventAggregator;

		public ICommand UpdateCommand { get; private set; }

		private void OnUpdate()
		{
			Player1Port += 1;

			//this.eventAggregator.GetEvent<UpdateEvent>().Publish(Player1Port.ToString());
		}

		private bool CanUpdate()
		{
			return true;
		}

		public ICommand ResetScoresCommand { get; private set; }

		private void OnResetScores()
		{
			Player1Score = 0;
			Player2Score = 0;
		}

		private bool CanResetScores()
		{
			return (this.player1Score != 0 || this.player2Score != 0);
		}

		public ICommand SwapPlayersCommand { get; private set; }

		private void OnSwapPlayers()
		{
			string tempName = Player1Name;
			Player1Name = Player2Name;
			Player2Name = tempName;

			int tempScore = Player1Score;
			Player1Score = Player2Score;
			Player2Score = tempScore;

			Port tempPort = Player1Port;
			Player1Port = Player2Port;
			Player2Port = tempPort;
		}

		private bool CanSwapPlayers()
		{
			return true;
		}
	}
}
