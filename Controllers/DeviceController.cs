using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using System;

namespace ArduinoApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase, IDisposable
    {
        private SerialPort serialPort;
        private bool disposed = false;

        public DeviceController()
        {
            // Initialize the serial port (adjust COM port as necessary)
            serialPort = new SerialPort("COM4", 9600);
            serialPort.Open();
        }

        // Turn the fan on
        [HttpGet("fan/on")]
        public IActionResult TurnFanOn()
        {
            return SendSerialCommand("Fan:ON", "Fan turned ON");
        }

        // Turn the fan off
        [HttpGet("fan/off")]
        public IActionResult TurnFanOff()
        {
            return SendSerialCommand("Fan:OFF", "Fan turned OFF");
        }

        // Turn the light on
        [HttpGet("light/on")]
        public IActionResult TurnLightOn()
        {
            return SendSerialCommand("Light:ON", "Light turned ON");
        }

        // Turn the light off
        [HttpGet("light/off")]
        public IActionResult TurnLightOff()
        {
            return SendSerialCommand("Light:OFF", "Light turned OFF");
        }

        // Turn the buzzer on
        [HttpGet("buzzer/on")]
        public IActionResult TurnBuzzerOn()
        {
            return SendSerialCommand("Buzzer:ON", "Buzzer turned ON");
        }

        // Turn the buzzer off
        [HttpGet("buzzer/off")]
        public IActionResult TurnBuzzerOff()
        {
            return SendSerialCommand("Buzzer:OFF", "Buzzer turned OFF");
        }

        // Open the door
        [HttpGet("door/open")]
        public IActionResult OpenDoor()
        {
            return SendSerialCommand("Door:ON", "Door opened");
        }

        // Close the door
        [HttpGet("door/close")]
        public IActionResult CloseDoor()
        {
            return SendSerialCommand("Door:OFF", "Door closed");
        }

        // Lockdown mode ON
        [HttpGet("lockdown/on")]
        public IActionResult LockdownOn()
        {
            try
            {
                // Send commands for lockdown on
                serialPort.WriteLine("Door:OFF"); // Close the door
                serialPort.WriteLine("Buzzer:ON"); // Turn on the buzzer
                serialPort.WriteLine("Light:ON"); // Turn on the light
                serialPort.WriteLine("Fan:ON"); // Turn on the fan
                return Ok("Lockdown activated: Door closed, Buzzer ON, Light ON, Fan ON");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during lockdown activation: {ex.Message}");
            }
        }

        // Lockdown mode OFF
        [HttpGet("lockdown/off")]
        public IActionResult LockdownOff()
        {
            try
            {
                // Send commands for lockdown off
                serialPort.WriteLine("Door:ON"); // Open the door
                serialPort.WriteLine("Buzzer:OFF"); // Turn off the buzzer
                serialPort.WriteLine("Light:OFF"); // Turn off the light
                serialPort.WriteLine("Fan:OFF"); // Turn off the fan
                return Ok("Lockdown deactivated: Door opened, Buzzer OFF, Light OFF, Fan OFF");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during lockdown deactivation: {ex.Message}");
            }
        }

        // Get status of all devices (example response)
        [HttpGet("status/all")]
        public IActionResult GetAllDeviceStatus()
        {
            // You can retrieve real-time status from Arduino if needed
            var deviceStatus = new[]
            {
                new { name = "fan", status = false },  // Replace with actual status
                new { name = "door controller", status = false },  // Replace with actual status
                new { name = "light", status = false },  // Replace with actual status
                new { name = "buzzer", status = false }  // Replace with actual status
            };

            return Ok(deviceStatus);
        }

        // Helper method to send commands to Arduino and return a response
        private IActionResult SendSerialCommand(string command, string successMessage)
        {
            try
            {
                serialPort.WriteLine(command);
                return Ok(successMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Dispose pattern for cleaning up resources
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    serialPort?.Close();
                    serialPort?.Dispose();
                }

                disposed = true;
            }
        }

        // Public implementation of Dispose pattern callable by consumers
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
