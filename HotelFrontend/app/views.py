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

# 忽略 HTTPS 自签名证书警告
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


def home(request):
    return render(request, 'app/home.html')  # 确保有对应的模板文件

#Hotle 首页
def hotel_home(request):
    return render(request, 'app/hotel_home.html')


def hotel_list(request):
    api_url = 'https://localhost:7256/api/hotels'  # 替换为您的后端 API 地址
    try:
        response = requests.get(api_url, verify=False)  # 禁用 SSL 验证（仅用于本地开发）
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
    # 固定的酒店 ID
    hotel_id = 11  # 要更新的酒店 ID
    api_url = f"https://localhost:7256/api/hotels/{hotel_id}"  # 后端 API URL

    if request.method == "POST":
        # 从前端表单获取数据
        name = request.POST.get("name")
        address = request.POST.get("address")
        city = request.POST.get("city")
        state = request.POST.get("state")
        zip_code = request.POST.get("zipCode")
        phone_number = request.POST.get("phoneNumber")
        email = request.POST.get("email")
        website = request.POST.get("website")

        # 创建请求体并包含 `id`
        hotel_data = {
            "id": hotel_id,  # 必须包含 ID 字段
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
            # 发送 PUT 请求到后端
            response = requests.put(api_url, json=hotel_data, verify=False)

            # 检查响应状态码
            if response.status_code == 200:
                return JsonResponse({"message": "Hotel updated successfully!"}, status=200)
            else:
                return JsonResponse(
                    {"error": response.text, "status_code": response.status_code},
                    status=response.status_code,
                )
        except Exception as e:
            return JsonResponse({"error": str(e)}, status=500)

    # 如果是 GET 请求，则渲染表单页面
    return render(request, "app/update_hotel.html", {"id": hotel_id})

# Booking 首页
def booking_home(request):
    return render(request, 'app/booking_home.html')


# 创建预订
def create_booking(request):
    if request.method == 'POST':
        # 从表单中获取数据
        room_number = request.POST.get('roomNumber')
        customer_name = request.POST.get('customerName')
        customer_email = request.POST.get('customerEmail')
        customer_phone = request.POST.get('customerPhone')
        check_in_date = request.POST.get('checkInDate')
        check_out_date = request.POST.get('checkOutDate')

        # 构造后端 API 请求数据
        booking_data = {
            "roomNumber": room_number,
            "customerName": customer_name,
            "customerEmail": customer_email,
            "customerPhone": customer_phone,
            "checkInDate": check_in_date,
            "checkOutDate": check_out_date,
        }

        # 发送 POST 请求到后端 API
        api_url = 'https://localhost:7256/api/bookings'  # 替换为你的后端 API 地址
        response = requests.post(api_url, json=booking_data, verify=False)

        # 检查后端响应状态码
        if response.status_code == 201:  # 201表示创建成功
            return HttpResponseRedirect('/booking/')  # 跳转到 Booking 管理页面
        else:
            return render(request, 'app/create_booking.html', {'error': 'Failed to create booking the room is booked'})

    # 如果是 GET 请求，渲染表单
    return render(request, 'app/create_booking.html')

# 删除预订
def delete_booking(request):
    if request.method == 'POST':
        # 获取表单中的房间号
        room_number = request.POST.get('roomNumber')
        if not room_number:
            return render(request, 'app/delete_booking.html', {'error': 'Room number is required.'})

        # 调用后端 API 删除预订
        api_url = f'https://localhost:7256/api/Bookings/room/{room_number}'
        try:
            response = requests.delete(api_url, verify=False)
            if response.status_code == 204:
                # 删除成功
                return render(request, 'app/delete_booking.html', {'message': 'Booking successfully deleted.'})
            elif response.status_code == 404:
                # 未找到预订记录
                return render(request, 'app/delete_booking.html', {'error': 'No booking found for the specified room number.'})
            else:
                # 处理其他错误
                return render(request, 'app/delete_booking.html', {'error': f'Failed to delete booking. Status code: {response.status_code}'})
        except requests.RequestException as e:
            # 捕获请求异常
            return render(request, 'app/delete_booking.html', {'error': f'An error occurred: {e}'})

    # 如果是 GET 请求，则渲染空表单
    return render(request, 'app/delete_booking.html')

def all_bookings(request):
   
    try:
        response = requests.get('https://localhost:7256/api/Bookings', verify=False)
        #print(response.text)  # 打印返回的原始数据
        bookings = response.json()  # 尝试解析 JSON
    except Exception as e:
        return render(request, 'error.html', {'error_message': str(e)})

    return render(request, 'app/booking_all.html', {'bookings': bookings})

# 查询预订
def view_booking_by_room(request):
    bookings = None
    error = None

    if request.method == "POST":
        room_number = request.POST.get('roomNumber')

        if room_number:
            # 后端 API URL
            api_url = f"https://localhost:7256/api/bookings/room/{room_number}"  # 假设后端支持通过房间号查询
            try:
                # 向后端发送 GET 请求
                response = requests.get(api_url, verify=False)
                if response.status_code == 200:
                    bookings = response.json()  # 获取预订信息
                else:
                    error = f"Error: {response.status_code} - {response.text}"
            except Exception as e:
                error = f"An error occurred: {e}"

    return render(request, 'app/view_booking.html', {'booking': bookings, 'error': error})

# 修改预订
# 更新 Booking 的视图
def update_booking(request):
    if request.method == 'POST':
        # 获取表单中的数据
        room_number = request.POST.get('roomNumber')
        customer_name = request.POST.get('customerName')
        customer_email = request.POST.get('customerEmail')
        customer_phone = request.POST.get('customerPhone')
        check_in_date = request.POST.get('checkInDate')
        check_out_date = request.POST.get('checkOutDate')

        # 构建 PATCH 数据
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

        # 检查是否有更新数据
        if not patch_data:
            return JsonResponse({"error": "No fields to update"}, status=400)

        # 调用后端 API
        api_url = f"https://localhost:7256/api/Bookings/room/{room_number}"  # 使用 HTTPS
        headers = {'Content-Type': 'application/json-patch+json'}

        try:
            response = requests.patch(api_url, json=patch_data, headers=headers, verify=False)
            if response.status_code == 200:
                return JsonResponse({"success": "Booking updated successfully"}, status=200)
            else:
                return JsonResponse(response.json(), status=response.status_code)
        except requests.RequestException as e:
            return JsonResponse({"error": str(e)}, status=500)

    # 处理 GET 请求，返回 HTML 页面
    return render(request, 'app/update_booking.html')

# Room 首页
def room_home(request):
    return render(request, 'app/room_home.html')

# 列出所有房间
def list_rooms(request):
    response = requests.get('https://localhost:7256/api/rooms', verify=False)
    rooms = response.json() if response.status_code == 200 else []
    return render(request, 'app/list_rooms.html', {'rooms': rooms})

# 修改房间信息
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

# 查询指定房间号
def view_room(request):
    room = None
    error = None

    if request.method == 'POST':
        room_number = request.POST.get('roomNumber')

        if room_number:
            # 调用后端 API 查询房间信息
            api_url = f'https://localhost:7256/api/rooms/{room_number}'
            try:
                response = requests.get(api_url, verify=False)
                if response.status_code == 200:
                    room = response.json()  # 获取房间信息
                elif response.status_code == 404:
                    error = "Room not found. Please check the room number and try again."
                else:
                    error = f"Error: Unable to fetch room details. Status code: {response.status_code}"
            except requests.RequestException as e:
                error = f"An error occurred while fetching room details: {e}"

    # 调试打印
    print("Room:", room)
    print("Error:", error)

    return render(request, 'app/view_room.html', {'room': room, 'error': error})





# 查询所有可用房间
def available_rooms(request):
    response = requests.get('https://localhost:7256/api/rooms', verify=False)  # 获取所有房间数据
    rooms = response.json() if response.status_code == 200 else []

    # 仅筛选出 Status 为 "Available" 的房间
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
