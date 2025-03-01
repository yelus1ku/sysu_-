"""
Definition of urls for HotelFrontend.
"""

from datetime import datetime
from django.urls import path, include  # 导入 include
from django.contrib import admin
from django.contrib.auth.views import LoginView, LogoutView
from app import forms, views


urlpatterns = [
     path('', views.home, name='home'),  # 主页面
    path('booking/', views.booking_home, name='booking_home'),
    path('room/', views.room_home, name='room_home'),
    path('hotels/', views.hotel_home, name='hotel_home'),  # Hotel 首页

    path('hotels/list/', views.hotel_list, name='hotel_list'),  # Hotel 列表页面
    path('hotels/update-hotel/', views.update_hotel, name='update_hotel'),
    # Booking模块路径
    path('booking/create/', views.create_booking, name='create_booking'),
    #path('booking/delete/', views.delete_booking, name='delete_booking'),
    path('delete-booking/', views.delete_booking, name='delete_booking'),
    path('booking/view/', views.view_booking_by_room, name='view_booking'),
    path('booking/update/', views.update_booking, name='update_booking'),
    path('all_bookings/', views.all_bookings, name='all_bookings'),
    path('update_booking/', views.update_booking, name='update_booking'),
   # path('view-all-bookings/', views.view_all_bookings, name='view_all_bookings'),
    # Room模块路径
    path('room/list/', views.list_rooms, name='list_rooms'),
    path('room/updatebooking/', views.update_room, name='update_room'),
    path('room/view/', views.view_room, name='view_room'),
   # path('room/clear/', views.clear_room_info, name='clear_room_info'),
    path('room/available/', views.available_rooms, name='available_rooms'),


    #path('', views.home, name='home'),
    path('contact/', views.contact, name='contact'),
    path('about/', views.about, name='about'),
    path('login/',
         LoginView.as_view
         (
             template_name='app/login.html',
             authentication_form=forms.BootstrapAuthenticationForm,
             extra_context=
             {
                 'title': 'Log in',
                 'year' : datetime.now().year,
             }
         ),
         name='login'),
    path('logout/', LogoutView.as_view(next_page='/'), name='logout'),
    path('admin/', admin.site.urls),
     # 新增：添加 hotel 应用的路由
    
    

]

