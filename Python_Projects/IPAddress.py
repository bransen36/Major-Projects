import ipaddress

# Global Constants
usage = '''=== Network Information and Subnetting Tool Usage ===
This script provides two functions to analyze IP networks and create subnets.
1. networkInformation(ip_cidr):
   - Displays network details for a given IP address and subnet mask in CIDR notation.
   - Example usage:
       networkInformation('192.168.1.10/24')
   - Expected output:
       Original Network Information:
       IP Address: 192.168.1.10
       Subnet Mask: 255.255.255.0
       Subnet Mask (CIDR): /24
       Network Address: 192.168.1.0
       Broadcast Address: 192.168.1.255
       Number of Host: 254
       Valid Host Range: 192.168.1.1 - 192.168.1.254

2. creatingSubnets():
   - Prompts the user for an IP address, subnet mask, and number of subnets.
   - Displays the original network information and then the details of the created subnets.
   - Example interaction:
       Enter the IP address (e.g., 132.8.150.67): 132.8.150.67
       Enter the subnet mask in CIDR notation (e.g., /22): /22
       How many subnets would you like to create? (e.g., 4): 4
   - Expected output (abridged):
       Original Network Information:
       IP Address: 132.8.150.67
       Subnet Mask: 255.255.252.0
       Subnet Mask (CIDR): /22
       Network Address: 132.8.148.0
       Broadcast Address: 132.8.151.255
       Number of Host: 1022
       Valid Host Range: 132.8.148.1 - 132.8.151.254
       Subnet Information (divided into 4 subnets with new mask /24):
       Subnet 1:
       Network Address: 132.8.148.0
       Broadcast Address: 132.8.148.255
       Number of Host: 254
       Valid Host Range: 132.8.148.1 - 132.8.148.254
       ... (continues for Subnets 2-4)
'''

def display_network_info(IPaddress, subnetMaskNum):
    """Displays the network details for a given IP address and subnet mask in CIDR notation."""
    try:
        # Create ip_interface object
        network_info = ipaddress.ip_interface(f"{IPaddress}/{subnetMaskNum}")
        NetworkAddress = network_info.network
        subnetMask = network_info.netmask
        BroadcastAddress = NetworkAddress.broadcast_address
        NumOfHosts = len(list(NetworkAddress.hosts()))  # Total hosts
        HostRange = f"{NetworkAddress.network_address + 1} - {BroadcastAddress - 1}"

        # Display network information
        print("\nOriginal Network Information:")
        print(f"IP Address: {IPaddress}")
        print(f"Subnet Mask: {subnetMask}")
        print(f"Subnet Mask (CIDR): {subnetMaskNum}")
        print(f"Network Address: {NetworkAddress}")
        print(f"Broadcast Address: {BroadcastAddress}")
        print(f"Number of Hosts: {NumOfHosts}")
        print(f"Valid Host Range: {HostRange}\n")

    except ValueError as e:
        print(f"Error: {e}. Please ensure the IP address and subnet mask are valid.")

def create_subnets():
    """Prompts the user for subnetting details and creates the required subnets."""
    try:
        IPaddress = input("Enter the IP address (e.g., 132.8.150.67): ").strip()
        subnetMaskNum = input("Enter the subnet mask in CIDR notation (e.g., /22): ").strip()
        numSubnets = int(input("How many subnets would you like to create?: ").strip())
        print()

        # Validate and compute network details
        network_info = ipaddress.ip_interface(f"{IPaddress}/{subnetMaskNum}").network
        subnet_list = list(network_info.subnets(prefixlen_diff=numSubnets))

        print(f"Original Network: {network_info}\n")
        print("Generated Subnets:")
        for i, subnet in enumerate(subnet_list, start=1):
            print(f"  Subnet {i}: {subnet}")

        print(f"\nTotal Subnets Created: {len(subnet_list)}\n")

    except ValueError as e:
        print(f"Error: {e}. Please ensure the input is valid.")
    except Exception as e:
        print(f"Unexpected error: {e}")

def main():
    """Main function to handle user interactions and program flow."""
    running = True

    while running:
        # Menu options
        menu = """=== Network Information and Subnetting Tool Menu ===
1. Display Network Information (networkInformation)
2. Create Subnets (creatingSubnets)
3. Show Usage Instructions (usage)
q. Quit
Enter your choice (1, 2, 3, or q): """

        choice = input(menu).strip().lower()
        print("="*50)

        if choice == '1':
            IPaddress = input("Enter the IP address (e.g., 132.8.150.67): ").strip()
            subnetMaskNum = input("Enter the subnet mask in CIDR notation (e.g., /22): ").strip()
            display_network_info(IPaddress, subnetMaskNum)

        elif choice == '2':
            create_subnets()

        elif choice == '3':
            print(usage)

        elif choice == 'q':
            running = False
            print("Thank you! Bye!")

        else:
            print("Invalid choice. Please enter 1, 2, 3, or q.\n")

if __name__ == "__main__":
    main()
