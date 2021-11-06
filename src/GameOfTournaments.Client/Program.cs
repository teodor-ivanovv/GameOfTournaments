namespace GameOfTournaments.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories.Models.Tournament;
    using GameOfTournaments.Data.Infrastructure;
    using Microsoft.AspNetCore.SignalR.Client;

    public class Program
    {
        public static async Task Main()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/TournamentsHub")
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += error =>
            {
                Console.WriteLine($"Connection closed. {error}");

                // await Task.Delay(new Random().Next(0,5) * 1000);
                // await connection.StartAsync();
                
                return Task.CompletedTask;
            };
            
            connection.On<OperationResult<List<TournamentViewModel>>>(
                "Snapshot",
                operationResult =>
                {
                    Console.WriteLine(operationResult.Success + " " + string.Join(", ", operationResult.Errors));
                    
                    if (operationResult.Object != null)
                    {
                        foreach (var tournament in operationResult.Object)
                            Console.WriteLine($"{tournament.Id} {tournament.Name} {tournament.Start} {tournament.Status}");
                    }
                });
  
            try
            {
                await connection.StartAsync();
                Console.WriteLine("Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            // async void sendButton_Click(object sender, RoutedEventArgs e)
            // {
            //     try
            //     {
            //         await connection.InvokeAsync("SendMessage", 
            //                                      userTextBox.Text, messageTextBox.Text);
            //     }
            //     catch (Exception ex)
            //     {                
            //         messagesList.Items.Add(ex.Message);                
            //     }
            // }
            
            Thread.Sleep(-1);
        }
    }
}