"""
Definition of views.
"""

from datetime import datetime
import requests
from django.shortcuts import render,redirect
from django.shortcuts import render, get_object_or_404
from django.http import JsonResponse
from django.http import HttpResponseRedirect
from django.http import HttpRequest
from django.shortcuts import render
import urllib3

import requests
from django.shortcuts import render
from django.conf import settings

# ���� HTTPS ��ǩ��֤�龯��
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


def home(request):
    return render(request, 'app/home.html')  # ȷ���ж�Ӧ��ģ���ļ�

#Hotle ��ҳ
def hotel_home(request):
    return render(request, 'app/hotel_home.html')


def hotel_list(request):
    api_url = 'https://localhost:7256/api/hotels'  # �滻Ϊ���ĺ�� API ��ַ
    try:
        response = requests.get(api_url, verify=False)  # ���� SSL ��֤�������ڱ��ؿ�����
        if response.status_code == 200:
            hotels = response.json()
        else:
            hotels = []
    except requests.RequestException as e:
        print(f"Error fetching hotel data: {e}")
        hotels = []

    return render(request, 'app/hotel_list.html', {'hotels': hotels})

def update_hotel(request):
    import requests
from django.http import JsonResponse
from django.shortcuts import render

def update_hotel(request):
    # �̶��ľƵ� ID
    hotel_id = 11  # Ҫ���µľƵ� ID
    api_url = f"https://localhost:7256/api/hotels/{hotel_id}"  # ��� API URL

    if request.method == "POST":
        # ��ǰ�˱���ȡ����
        name = request.POST.get("name")
        address = request.POST.get("address")
        city = request.POST.get("city")
        state = request.POST.get("state")
        zip_code = request.POST.get("zipCode")
        phone_number = request.POST.get("phoneNumber")
        email = request.POST.get("email")
        website = request.POST.get("website")

        # ���������岢���� `id`
        hotel_data = {
            "id": hotel_id,  # ������� ID �ֶ�
            "name": name,
            "address": address,
            "city": city,
            "state": state,
            "zipCode": zip_code,
            "phoneNumber": phone_number,
            "email": email,
            "website": website,
        }

        try:
            # ���� PUT ���󵽺��
            response = requests.put(api_url, json=hotel_data, verify=False)

            # �����Ӧ״̬��
            if response.status_code == 200:
                return JsonResponse({"message": "Hotel updated successfully!"}, status=200)
            else:
                return JsonResponse(
                    {"error": response.text, "status_code": response.status_code},
                    status=response.status_code,
                )
        except Exception as e:
            return JsonResponse({"error": str(e)}, status=500)

    # ����� GET ��������Ⱦ��ҳ��
    return render(request, "app/update_hotel.html", {"id": hotel_id})

# Booking ��ҳ
def booking_home(request):
    return render(request, 'app/booking_home.html')


# ����Ԥ��
def create_booking(request):
    if request.method == 'POST':
        # �ӱ��л�ȡ����
        room_number = request.POST.get('roomNumber')
        customer_name = request.POST.get('customerName')
        customer_email = request.POST.get('customerEmail')
        customer_phone = request.POST.get('customerPhone')
        check_in_date = request.POST.get('checkInDate')
        check_out_date = request.POST.get('checkOutDate')

        # ������ API ��������
        booking_data = {
            "roomNumber": room_number,
            "customerName": customer_name,
            "customerEmail": customer_email,
            "customerPhone": customer_phone,
            "checkInDate": check_in_date,
            "checkOutDate": check_out_date,
        }

        # ���� POST ���󵽺�� API
        api_url = 'https://localhost:7256/api/bookings'  # �滻Ϊ��ĺ�� API ��ַ
        response = requests.post(api_url, json=booking_data, verify=False)

        # �������Ӧ״̬��
        if response.status_code == 201:  # 201��ʾ�����ɹ�
            return HttpResponseRedirect('/booking/')  # ��ת�� Booking ����ҳ��
        else:
            return render(request, 'app/create_booking.html', {'error': 'Failed to create booking the room is booked'})

    # ����� GET ������Ⱦ��
    return render(request, 'app/create_booking.html')

# ɾ��Ԥ��
def delete_booking(request):
    if request.method == 'POST':
        # ��ȡ���еķ����
        room_number = request.POST.get('roomNumber')
        if not room_number:
            return render(request, 'app/delete_booking.html', {'error': 'Room number is required.'})

        # ���ú�� API ɾ��Ԥ��
        api_url = f'https://localhost:7256/api/Bookings/room/{room_number}'
        try:
            response = requests.delete(api_url, verify=False)
            if response.status_code == 204:
                # ɾ���ɹ�
                return render(request, 'app/delete_booking.html', {'message': 'Booking successfully deleted.'})
            elif response.status_code == 404:
                # δ�ҵ�Ԥ����¼
                return render(request, 'app/delete_booking.html', {'error': 'No booking found for the specified room number.'})
            else:
                # ������������
                return render(request, 'app/delete_booking.html', {'error': f'Failed to delete booking. Status code: {response.status_code}'})
        except requests.RequestException as e:
            # ���������쳣
            return render(request, 'app/delete_booking.html', {'error': f'An error occurred: {e}'})

    # ����� GET ��������Ⱦ�ձ�
    return render(request, 'app/delete_booking.html')

def all_bookings(request):
   
    try:
        response = requests.get('https://localhost:7256/api/Bookings', verify=False)
        #print(response.text)  # ��ӡ���ص�ԭʼ����
        bookings = response.json()  # ���Խ��� JSON
    except Exception as e:
        return render(request, 'error.html', {'error_message': str(e)})

    return render(request, 'app/booking_all.html', {'bookings': bookings})

# ��ѯԤ��
def view_booking_by_room(request):
    bookings = None
    error = None

    if request.method == "POST":
        room_number = request.POST.get('roomNumber')

        if room_number:
            # ��� API URL
            api_url = f"https://localhost:7256/api/bookings/room/{room_number}"  # ������֧��ͨ������Ų�ѯ
            try:
                # ���˷��� GET ����
                response = requests.get(api_url, verify=False)
                if response.status_code == 200:
                    bookings = response.json()  # ��ȡԤ����Ϣ
                else:
                    error = f"Error: {response.status_code} - {response.text}"
            except Exception as e:
                error = f"An error occurred: {e}"

    return render(request, 'app/view_booking.html', {'booking': bookings, 'error': error})

# �޸�Ԥ��
# ���� Booking ����ͼ
def update_booking(request):
    if request.method == 'POST':
        # ��ȡ���е�����
        room_number = request.POST.get('roomNumber')
        customer_name = request.POST.get('customerName')
        customer_email = request.POST.get('customerEmail')
        customer_phone = request.POST.get('customerPhone')
        check_in_date = request.POST.get('checkInDate')
        check_out_date = request.POST.get('checkOutDate')

        # ���� PATCH ����
        patch_data = []
        if customer_name:
            patch_data.append({"op": "replace", "path": "/customerName", "value": customer_name})
        if customer_email:
            patch_data.append({"op": "replace", "path": "/customerEmail", "value": customer_email})
        if customer_phone:
            patch_data.append({"op": "replace", "path": "/customerPhone", "value": customer_phone})
        if check_in_date:
            patch_data.append({"op": "replace", "path": "/checkInDate", "value": check_in_date})
        if check_out_date:
            patch_data.append({"op": "replace", "path": "/checkOutDate", "value": check_out_date})

        # ����Ƿ��и�������
        if not patch_data:
            return JsonResponse({"error": "No fields to update"}, status=400)

        # ���ú�� API
        api_url = f"https://localhost:7256/api/Bookings/room/{room_number}"  # ʹ�� HTTPS
        headers = {'Content-Type': 'application/json-patch+json'}

        try:
            response = requests.patch(api_url, json=patch_data, headers=headers, verify=False)
            if response.status_code == 200:
                return JsonResponse({"success": "Booking updated successfully"}, status=200)
            else:
                return JsonResponse(response.json(), status=response.status_code)
        except requests.RequestException as e:
            return JsonResponse({"error": str(e)}, status=500)

    # ���� GET ���󣬷��� HTML ҳ��
    return render(request, 'app/update_booking.html')

# Room ��ҳ
def room_home(request):
    return render(request, 'app/room_home.html')

# �г����з���
def list_rooms(request):
    response = requests.get('https://localhost:7256/api/rooms', verify=False)
    rooms = response.json() if response.status_code == 200 else []
    return render(request, 'app/list_rooms.html', {'rooms': rooms})

# �޸ķ�����Ϣ
def update_room(request):
    if request.method == 'POST':
        room_number = request.POST['roomNumber']
        data = {
            'roomType': request.POST['roomType'],
            'roomPrice': request.POST['roomPrice'],
        }
        response = requests.put(f'https://localhost:7256/api/rooms/{room_number}', json=data, verify=False)
        if response.status_code == 200:
            return redirect('room_home')
    return render(request, 'app/update_room.html')

# ��ѯָ�������
def view_room(request):
    room = None
    error = None

    if request.method == 'POST':
        room_number = request.POST.get('roomNumber')

        if room_number:
            # ���ú�� API ��ѯ������Ϣ
            api_url = f'https://localhost:7256/api/rooms/{room_number}'
            try:
                response = requests.get(api_url, verify=False)
                if response.status_code == 200:
                    room = response.json()  # ��ȡ������Ϣ
                elif response.status_code == 404:
                    error = "Room not found. Please check the room number and try again."
                else:
                    error = f"Error: Unable to fetch room details. Status code: {response.status_code}"
            except requests.RequestException as e:
                error = f"An error occurred while fetching room details: {e}"

    # ���Դ�ӡ
    print("Room:", room)
    print("Error:", error)

    return render(request, 'app/view_room.html', {'room': room, 'error': error})





# ��ѯ���п��÷���
def available_rooms(request):
    response = requests.get('https://localhost:7256/api/rooms', verify=False)  # ��ȡ���з�������
    rooms = response.json() if response.status_code == 200 else []

    # ��ɸѡ�� Status Ϊ "Available" �ķ���
    available_rooms = [room for room in rooms if room.get('status') == 'Available']

    return render(request, 'app/available_rooms.html', {'rooms': available_rooms})





def contact(request):
    """Renders the contact page."""
    assert isinstance(request, HttpRequest)
    return render(
        request,
        'app/contact.html',
        {
            'title':'Contact',
            'message':'Your contact page.',
            'year':datetime.now().year,
        }
    )

def about(request):
    """Renders the about page."""
    assert isinstance(request, HttpRequest)
    return render(
        request,
        'app/about.html',
        {
            'title':'About',
            'message':'Your application description page.',
            'year':datetime.now().year,
        }
    )
