import ipaddress

running = True
usage = '''=== Network Information and Subnetting Tool Usage ===\n
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
    ... (continues for Subnets 2-4)\n\n'''

def findNetInfo(IPaddress, subnetMaskNum):
    ComputerAddress = ipaddress.ip_interface(IPaddress+'/'+subnetMaskNum)
    NetworkAddress = ComputerAddress.network
    subnetMask = ComputerAddress.netmask
    BroadcastAddress = NetworkAddress.broadcast_address
    NumOfHosts = len(list(NetworkAddress.hosts()))
    HostRange = '{0} - {1}'.format((NetworkAddress.network_address +1), (BroadcastAddress -1))
    print("Original Network Information:\nIP Address:", IPaddress, "\nSubnet Mask:", subnetMask , "\nSubnet Mask (CIDR):", subnetMaskNum, "\nNetwork Address:", NetworkAddress, "\nBroadcast Address:", BroadcastAddress, "\nNumber of Host:", NumOfHosts, "\nValid Host Range:", HostRange, "\n")

while running:
    menu = "=== Network Information and Subnetting Tool Menu ===\n 1. Display Network Information (networkInformation)\n 2. Create Subnets (creatingSubnets)\n 3. Show Usage Instructions (usage)\n q. Quit\n\nEnter your choice (1, 2, 3, or q): "

    choice = input(menu)
    print("====================================================\n")

    if choice == '1':
        IPaddress = input("Enter the IP address (e.g, 132.8.150.67): ")
        subnetMaskNum = input("Enter the subnet mask in CIDR notation (e.g., 22): ")
        findNetInfo(IPaddress, subnetMaskNum)
    elif choice == '2':
        import ipaddress

        IPaddress = input("Enter the IP address (e.g., 132.8.150.67): ")
        subnetMaskNum = input("Enter the subnet mask in CIDR notation (e.g., 22): ")
        numSubnets = int(input("How many subnet masks would you like to create: "))
        print()

        # Compute network information
        NetworkAddress = ipaddress.ip_interface(f"{IPaddress}/{subnetMaskNum}").network
        NewNetworkAddresses = list(NetworkAddress.subnets(prefixlen_diff=numSubnets))

        # Display results
        print("=== Subnet Information ===")
        print(f"Original Network: {NetworkAddress}\n")
        print("Generated Subnets:")

        for i, subnet in enumerate(NewNetworkAddresses, start=1):
            print(f"  {i}. {subnet}")

        print("\nTotal Subnets Created:", len(NewNetworkAddresses))
    elif choice == '3':
        print(usage)
    elif choice == 'q':
        running = False
        print("Thank you! Bye!")
    else:
        print("That wasn't one of the options, please try again.")

    