import {
  ChangeDetectionStrategy, Component, OnInit, ChangeDetectorRef,
  TemplateRef, ContentChild, ViewChild
} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbToastrService, NbComponentStatus, NbDialogContainerComponent } from '@nebular/theme';
import {
  CalendarOptions, FullCalendarComponent, EventClickArg, EventApi,
  DateSelectArg, EventHoveringArg, EventContentArg
} from '@fullcalendar/angular';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'ngx-reservationnew',
  templateUrl: './reservationnew.component.html',
  styleUrls: ['./reservationnew.component.scss']
})
export class ReservationnewComponent implements OnInit {
  displayMode = 'INVENTORY';
  isLoadingInventories = false;
  inventories: any[];
  channels: any[];
  agents: any[];
  doctypes: any[];
  alldoctypes: any[];
  resdocs: any[];
  deleteDialogRef: any;
  reserveDialogRef: any;
  dialogRef: any;
  updatedialogRef: any;
  reservations: any[];
  rooms: any[];
  room = new Room();
  reservData = new ReservData();
  roomData = new RoomData();
  defaultPropertyId: string = "0";
  defaultDocTypeId: string = "0";
  deleteId: any;
  docfiles: any;
  isLoading = false;
  loadingSave = false;
  loadingDelete = false;
  isLoadingRooms = false;
  isLoadingReservations = false;
  loadingDocSave = false;
  loadingStatusSave = false;
  properties: any[];
  roomDetails: RoomData[] = [];
  roominventory: InvData[] = [];
  currentEvents: EventApi[] = [];
  token: any;
  discDisabled: boolean = false;
  property = new Property();
  @ViewChild('calendar') calendarComponent: FullCalendarComponent;
  @ViewChild('reserveDialogNew') reserveDialogNew: TemplateRef<any>;
  @ViewChild('reserveDialog') reserveDialog: TemplateRef<any>;
  @ViewChild('eventtooltip') eventtooltip: TemplateRef<any>;
  @ViewChild('paxDetails ') paxDetails: TemplateRef<any>;

  // autocomplete
  options: any[];
  filteredOptions$: Observable<any[]>;
  defaultValue: any;
  selectedPropertyName: string;
  isDisabled: boolean = true;
  countries: any[];
  user: any;
  mode: string;

  createFormData = new FormData();
  foundFile = false;
  defaultCreateDocTypeId: string = "0";
  createdoctypes: any[];
  //createDocFiles: any[];
  createresdocs: createresdocsData[] = [];


  @ViewChild('autoInput') input;

  calendarOptions: CalendarOptions = {
    displayEventTime: false,
    height: '100%',
    contentHeight: 'auto',
    headerToolbar: {
      left: 'today prev,next',
      center: 'title',
      right: ''
    },
    views: {
      dayGridMonth: { // name of view
        titleFormat: { year: 'numeric', month: 'short' },
        // other view-specific options here
      }
    },
    initialDate: new Date(),
    // validRange: {
    //  start: new Date(),
    // },
    navLinks: false, // can click day/week names to navigate views
    selectable: true,
    selectMirror: true,
    select: this.handleDateSelect.bind(this),
    eventClick: this.handleEventClick.bind(this),
    eventsSet: this.handleEvents.bind(this),
    editable: false,
    dayMaxEvents: true, // allow "more" link when too many events
    events: [
    ]
  };

  private filter(value: string): any[] {
    const filterValue = value.toLowerCase();
    return this.options.filter(optionValue => optionValue.title.toLowerCase().includes(filterValue));
  }

  getFilteredOptions(value: string): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.filter(filterString)),
    );
  }

  onChange() {

    // this.input.nativeElement.setAttribute("area-expanded", "true");
    this.filteredOptions$ = this.getFilteredOptions(this.input.nativeElement.value);
  }

  onSelectionChange($event) {

    // console.log(JSON.stringify($event));

    this.defaultPropertyId = $event.id;
    this.selectedPropertyName = $event.title.toString().trim();
    localStorage["default-reservation-property"] = this.defaultPropertyId;
    localStorage["default-property-name"] = this.selectedPropertyName;
    this.loadProperty();
    this.filteredOptions$ = this.getFilteredOptions(this.selectedPropertyName);


    this.reservData = new ReservData();
    this.reservData.id = 0;
    // this.reservData.propertyId = this.property.id.toString();
  }

  loadProperty() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.defaultPropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
          this.rooms = [];
          this.room = new Room();
          this.roomData = new RoomData();
          this.roomDetails = [];
          this.loadRooms();
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadRooms() {

    this.isLoadingRooms = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.get('api/property/get-room-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {


          if (res.data.length > 0) {
            this.rooms = res.data;

          }
        }
        this.isLoadingRooms = false;
      },
        error => {

          this.isLoadingRooms = false;
          console.log('api/property/get-room-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }
  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {


    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      this.user = tokenData.getPayload();
    });

  }

  ngOnInit(): void {
    this.loadProperties();
    this.loadProperty();
    this.loadBookingChannels();
    this.loadDocTypes();
    this.loadCountries();
    this.loadBookingAgents();
  }

  private loadProperties() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-active-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;

            this.options = this.properties;
            this.filteredOptions$ = of(this.options);
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/property/get-active-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  private loadBookingAgents() {
    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/user/get-list'
      , { headers: headers }).subscribe((res: any) => {
        this.isLoading = false;
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.agents = res.data;
            //console.log(JSON.stringify(this.agents));
          }
        }
      },
        error => {
          this.isLoading = false;
          console.log('api/user/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  private loadBookingChannels() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/bookingchannel/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.channels = res.data;
            // console.log(JSON.stringify(this.channels));
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/bookingchannel/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadCountries() {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/location/get-country-list'
      , { headers: headers }).subscribe((res: any) => {



        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.countries = res.data;

          }
        }

      },
        error => {

          console.log('api/location/get-country-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  showBookings(evt) {
    var target = evt.target;
    if (target.checked) {
      this.displayMode = 'BOOKINGS';
      this.loadReservations();
      this.roomDetails = [];
    } else {
    }
  }

  showInventory(evt) {
    var target = evt.target;
    if (target.checked) {
      this.displayMode = 'INVENTORY';
      this.loadInventories();
    } else {

    }
  }

  loadReservations() {

    this.reservations = [];
    this.displayReservations();
    this.isLoadingReservations = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reservation/get-list-new?propertyId=' + this.property.id + '&roomId=' + this.room.id
      , { headers: headers }).subscribe((res: any) => {

        // console.log(JSON.stringify(res.data));

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.reservations = res.data;
          }
          if (this.displayMode == 'INVENTORY') {
            this.displayInventories();
          }
          else if (this.displayMode == 'BOOKINGS') {
            this.displayReservations();
          }
        }
        this.isLoadingReservations = false;
      },
        error => {

          this.isLoadingReservations = false;
          console.log('api/reservation/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  displayReservations() {
    var events = [];
    for (let i = 0; i < this.reservations.length; i++) {
      if (this.reservations[i].validation_status == 'YES') {
        var color = "#EA0043";
      }
      else {
        var color = "#3788d8";
      }
      var res_status = "";
      if (this.reservations[i].res_status == 'COMPLETED') {
        res_status = "[OK] ";
      }
      if (this.reservations[i].isHostBooking == 'Y') {
        color = "#800080";
        res_status = "";
      } else {
        if (this.reservations[i].bookingChannelId == 45) {
          color = "#008000";
          res_status = "";
        }
      }
      events.push(
        {
          id: this.reservations[i].id,
          title: res_status + this.reservations[i].custName,
          start: this.reservations[i].fromDate,
          end: this.reservations[i].toDate,
          color: color,
          allDay: true
        });
    }
    this.calendarOptions.events = events;
  }

  loadInventories() {

    this.isLoadingInventories = true;

    this.inventories = [];

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/inventory/get-list-room?propertyId=' + this.property.id
      + '&roomId=' + this.room.id
      , { headers: headers }).subscribe((res: any) => {

        // console.log(JSON.stringify(res));

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.inventories = res.data;
          }

          this.loadReservations();
        }
        this.isLoadingInventories = false;
      },
        error => {

          this.isLoadingInventories = false;
          console.log('api/inventory/get-list-room', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  displayInventories() {
    var events = [];
    for (let i = 0; i < this.inventories.length; i++) {
      events.push(
        {
          id: this.inventories[i].id,
          title: this.inventories[i].totalQty - this.inventories[i].bookedQty + " Rooms",
          ExtraBed: this.inventories[i].extraBedPrice,
          Child: this.inventories[i].childPrice,
          OccRate: this.inventories[i].occrates,
          start: this.inventories[i].invDate,
          end: this.inventories[i].invDate,
          allDay: false,
          editable: false
        });

      events.push(
        {
          id: "P" + this.inventories[i].id,
          title: "RS " + this.inventories[i].price,
          ExtraBed: this.inventories[i].extraBedPrice,
          Child: this.inventories[i].childPrice,
          OccRate: this.inventories[i].occrates,
          start: this.inventories[i].invDate,
          end: this.inventories[i].invDate,
          allDay: false,
          editable: false
        });
    }
    this.calendarOptions.events = events;
  }

  selectRoom(item) {
    this.room = item;
    this.room.propertyId = parseInt(this.property.id);
    this.roomData = new RoomData();
    let current_date = new Date();
    this.roomData.fromDate = new Date();
    this.roomData.toDate = new Date(current_date.setDate(current_date.getDate() + 1));
    this.calendarOptions.events = [];
    this.loadInventories();
  }

  handleDateSelect(selectInfo: DateSelectArg) {

    var check = this.formatDate(selectInfo.startStr);
    var today = this.formatDate(new Date());

    if (check >= today) {

      // check if there is a reservation already
      var maxAvailableQty = this.checkIfInventory(selectInfo.startStr, selectInfo.endStr);
      if (maxAvailableQty > 0) {
        // const cust_name = prompt('Please enter customer name');
        const calendarApi = selectInfo.view.calendar;

        calendarApi.unselect(); // clear date selection

        this.reservData = new ReservData();
        this.reservData.maxAvailableQty = maxAvailableQty;
        this.reservData.propertyId = this.property.id.toString();
        this.reservData.advancepayment = 0;
        this.reservData.partpayment = 0;
        this.reservData.balancepayment = 0;

        this.dialogRef = this.dialogService.open(
          this.reserveDialogNew,
          { context: { title: 'Create Reservation' } });

        // this.reservData.custName = cust_name;
        // this.reservData.custEmail = "nasar@gmail.com";
        // this.reservData.custPhone = "12345";


        // if (cust_name) {


        // this.createReservation();

        // calendarApi.addEvent({
        //  title,
        //  start: selectInfo.startStr,
        //  end: selectInfo.endStr,
        //  allDay: selectInfo.allDay
        // });
        // }
      }

    }

  }

  handleEventClick(clickInfo: EventClickArg) {

    if (this.displayMode == 'BOOKINGS') {
      this.mode = 'UPDATE';
      this.reservData = new ReservData();
      this.reservData = this.reservations.find(_obj => _obj.id == clickInfo.event.id);
      if (this.reservData.booking_agent == "0") {
        this.reservData.booking_agent = this.user.id;
      }
      else {
        this.reservData.booking_agent = this.reservData.booking_agent.toString();
      }
      this.channelchange();
      // var maxAvailableQty = this.checkIfInventory(this.roomData.fromDate, this.roomData.toDate);
      //// console.log(this.reservData.noOfRooms);
      // this.reservData.maxAvailableQty = parseInt(this.reservData.noOfRooms) + maxAvailableQty;

      // this.roomData.fromDate = this.formatDate(this.roomData.fromDate);
      // this.roomData.toDate = this.formatDate(this.roomData.toDate);

      this.isLoadingReservations = true;
      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");
      this.http.get('api/reservation/get-reserve-rooms?Id=' + this.reservData.id
        , { headers: headers }).subscribe((res: any) => {

          // console.log(JSON.stringify(res.data));

          if (res.actionStatus == 'SUCCESS') {
            if (res.data.length > 0) {
              this.roomDetails = res.data;
            }
          }
          this.isLoadingReservations = false;
        },
          error => {

            this.isLoadingReservations = false;
            console.log('api/reservation/get-reserve-rooms', error)
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }
          });

      this.updatedialogRef = this.dialogService.open(
        this.reserveDialog,
        { context: { title: 'Update Reservation' } });

      this.loadDocs();
    }
    else if (this.displayMode == 'INVENTORY') {
      this.dialogRef = this.dialogService.open(
        this.eventtooltip,
        {
          context: {
            ebp: clickInfo.event.extendedProps.ExtraBed,
            cp: clickInfo.event.extendedProps.Child,
            occrate: clickInfo.event.extendedProps.OccRate
          }
        });
    }
  }

  handleEvents(events: EventApi[]) {
    this.currentEvents = events;
  }

  formatDate(date) {
    var d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
  }

  checkIfReservation(startStr, endStr) {

    const startDate = this.formatDate(startStr);
    const endDate = this.formatDate(endStr);

    for (let i = 0; i < this.reservations.length; i++) {
      const resFromDate = this.formatDate(this.reservations[i].fromDate);
      const resToDate = this.formatDate(this.reservations[i].toDate);

      if (((startDate >= resFromDate) && (startDate < resToDate))
        || ((endDate > resFromDate) && (endDate <= resToDate))) {
        return true;
      }
      else if (((resFromDate >= startDate) && (resFromDate < endDate))
        || ((resToDate > startDate) && (resToDate <= endDate))) {
        return true;
      }
    }
    return false;
  }

  checkIfInventory(startStr, endStr) {

    const startDate = this.formatDate(startStr);
    const endDate = this.formatDate(endStr);

    var date1Tmp = new Date(startDate);
    var date2Tmp = new Date(endDate);


    var minQty = this.getAvailableQty(date1Tmp);
    date1Tmp.setDate(date1Tmp.getDate() + 1);
    while (date1Tmp < date2Tmp) {
      var minQtyTmp = this.getAvailableQty(date1Tmp);
      if (minQtyTmp < minQty) {
        minQty = minQtyTmp;
      }
      date1Tmp.setDate(date1Tmp.getDate() + 1);
    }


    return minQty;
  }

  getAvailableQty(dateTmp) {
    for (let i = 0; i < this.inventories.length; i++) {

      var invDate = new Date(this.inventories[i].invDate);
      // console.log(dateTmp);
      // console.log(invDate);
      if (this.formatDate(dateTmp) == this.formatDate(invDate)) {
        return this.inventories[i].totalQty - this.inventories[i].bookedQty;
      }
    }

    return 0;
  }

  popclose() {

    this.dialogRef.close();
  }

  validateCheckinCheckoutDates(checkinDate, checkoutDate): boolean {

    //// Ensure that checkinDate and checkoutDate are valid Date objects
    // if (!(checkinDate instanceof Date) || !(checkoutDate instanceof Date)) {
    //  this.toast('Error', 'Please select valid dates!', 'danger');
    //  return false;
    // }

    // Check if checkinDate is before checkoutDate

    if (checkinDate >= checkoutDate) {
      this.toast('Error', 'Check In date should be less than Check Out date!', 'danger');
      return false;
    }

    if (this.user.role != 'ADMIN') {
      // Check if the checkinDate is in the future (optional)
      const currentDate = new Date();
      if (checkinDate.setHours(0, 0, 0, 0) < currentDate.setHours(0, 0, 0, 0)) {
        this.toast('Error', 'Check In date should not be less than current date!', 'danger');
        return false;
      }
    }

    // All checks passed, the dates are valid
    return true;
  }

  onAddClick() {
    this.mode = 'CREATE';
    this.loadingSave = true;
    this.roomData.fromDate.setMinutes(this.roomData.fromDate.getMinutes() - this.roomData.fromDate.getTimezoneOffset());
    this.roomData.toDate.setMinutes(this.roomData.toDate.getMinutes() - this.roomData.toDate.getTimezoneOffset());

    var proceed = true;

    if (this.validateCheckinCheckoutDates(new Date(this.roomData.fromDate), new Date(this.roomData.toDate))) {

      var date1Tmp = new Date(this.roomData.fromDate);
      var date2Tmp = new Date(this.roomData.toDate);



      while (date1Tmp < date2Tmp) {

        var maxAvailableQty = this.checkIfInventory(date1Tmp, date1Tmp);
        let selectedqty = this.roomDetails.filter(c => c.roomId == this.room.id.toString() && new Date(c.fromDate) <= new Date(date1Tmp) && new Date(date1Tmp) < new Date(c.toDate)).length;

        if (maxAvailableQty - selectedqty > 0) {
          proceed = true
        }
        else {
          proceed = false;
          this.toast('Error', 'There is no selected rooms left!', 'danger');
          this.loadingSave = false;
          return false;
        }

        date1Tmp.setDate(date1Tmp.getDate() + 1);
      }


      //var maxAvailableQty = this.checkIfInventory(new Date(this.roomData.fromDate), new Date(this.roomData.toDate));
      //let selectedqty = this.roomDetails.filter(c => c.roomId == this.room.id.toString()).length;
      //alert(maxAvailableQty);
      //alert(selectedqty);
      //if (maxAvailableQty - selectedqty > 0) {
      this.roomData.roomId = this.room.id.toString();
      this.roomData.room = this.room.title.toString();
      this.roomData.years06 = 0;
      this.roomData.years612 = 0;
      this.roomData.years12 = 0;
      this.roomData.noOfGuests = 0;
      this.roomData.amount = 0;
      this.roomData.discount = 0;
      this.roomData.newbamt = 0;


      this.getroominventory();
      this.hikechange();

      this.dialogRef = this.dialogService.open(
        this.paxDetails,
        { context: { title: 'Add Room Details' } });
      this.loadingSave = false;

      //}
      //else {
      //  this.toast('Error', 'There is no selected rooms left!', 'danger');
      //  this.loadingSave = false;
      //  return false;
      //}
    }
    this.loadingSave = false;

  }

  getroominventory() {
    this.isLoadingReservations = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/inventory/get-room-inventory?propertyId=' + this.property.id + '&roomId=' + this.room.id
      + '&from=' + this.formatDate(this.roomData.fromDate) + '&to=' + this.formatDate(this.roomData.toDate)
      , { headers: headers }).subscribe((res: any) => {


        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.roominventory = res.data;

          }
        }
        this.isLoadingReservations = false;
      },
        error => {

          this.isLoadingReservations = false;
          console.log('api/inventory/get-room-inventory', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  paxchange() {
    let inv = this.roominventory.find((c, i) => this.formatDate(c.invDate)
      == this.formatDate(this.roomData.fromDate)
      && c.propertyId == this.property.id && c.roomId == this.roomData.roomId && i == 0);

    if (this.roomData.years12 > inv.maxadults) {
      this.roomData.years12 = 0;
      this.toast('Error', 'Maximum allowed adult count is ' + inv.maxadults + '!', 'danger');
      this.roomData.noOfGuests = this.roomData.years06 + this.roomData.years612 + this.roomData.years12;
      return false;
    }
    if (this.roomData.years612 > inv.maxchildren) {
      if (this.roomData.years612 > inv.maxchildren + inv.normalocc - this.roomData.years12) {

        this.roomData.years06 = 0
        this.roomData.years612 = 0
        this.toast('Error', 'Maximum allowed child count is '
          + (inv.maxchildren + inv.normalocc - this.roomData.years12).toString() + '!', 'danger');
        this.roomData.noOfGuests = this.roomData.years06 + this.roomData.years612 + this.roomData.years12;
        return false;
      }
    }
    this.roomData.noOfGuests = this.roomData.years06 + this.roomData.years612 + this.roomData.years12;
    this.calculation();
  }

  hikechange() {
    if (this.roomData.amount != this.roomData.newbamt) {
      this.discDisabled = true;
      this.roomData.discount = 0;
    }
    else {
      this.discDisabled = false;
    }
  }

  onpaxadd() {

    if (this.roomData.noOfGuests > 0) {
      if (this.mode == 'CREATE') {
        if (this.roomDetails.length == 0) {
          this.roomData.id = 1;
        }
        else {
          this.roomData.id = this.roomDetails[this.roomDetails.length - 1].id + 1;
        }
      }
      else {

        this.roomDetails = this.roomDetails.filter(_obj => _obj.id != this.roomData.id);
      }
      this.roomDetails.push(this.roomData);

      this.dialogRef.close();

      this.roomData = new RoomData();
      let current_date = new Date();
      this.roomData.fromDate = new Date(current_date);
      this.roomData.toDate = new Date(current_date.setDate(current_date.getDate() + 1));
      this.roomData.roomId = this.room.id.toString();

      this.discDisabled = false;
      this.paymentchange();
    }
    else {
      this.toast('Error', 'Atleast add one person in room!', 'danger');
      return false;
    }
  }

  calculation() {

    var adults = this.roomData.years12;
    var child = this.roomData.years612;
    var discount = this.roomData.discount;
    var eb = 0;
    var ebprice = 0;
    var cprice = 0;
    var occprice = 0;

    var dt = this.formatDate(this.roomData.fromDate);
    while (dt < this.formatDate(this.roomData.toDate)) {

      let ch = this.roominventory.find(c => this.formatDate(c.invDate)
        == this.formatDate(dt)
        && c.propertyId == this.property.id && c.roomId == this.roomData.roomId && c.occupancy == 'Child Price');
      let e = this.roominventory.find(c => this.formatDate(c.invDate)
        == this.formatDate(dt)
        && c.propertyId == this.property.id && c.roomId == this.roomData.roomId && c.occupancy == 'Extra Bed Price');

      var no = ch.normalocc;

      if (no < adults) {
        eb = adults - no;
        ebprice += (adults - no) * e.rate;
        let o = this.roominventory.find(c => this.formatDate(c.invDate)
          == this.formatDate(dt)
          && c.propertyId == this.property.id && c.roomId == this.roomData.roomId && c.occcount == no);
        occprice += o.rate;
      }
      else {
        let o = this.roominventory.find(c => this.formatDate(c.invDate)
          == this.formatDate(dt)
          && c.propertyId == this.property.id && c.roomId == this.roomData.roomId && c.occcount == adults);
        occprice += o.rate;
      }

      cprice += child * ch.rate;

      dt = this.formatDate(new Date(dt).setDate(new Date(dt).getDate() + 1));
    }

    this.roomData.amount = occprice + ebprice + cprice - discount;
    this.roomData.newbamt = this.roomData.amount;
    this.discDisabled = false;
  }

  deleterow(id) {
    this.roomDetails = this.roomDetails.filter(_obj => _obj.id != id);
  }

  checkout() {
    this.createFormData = new FormData();
    this.foundFile = false;

    this.reservData.booking_agent = this.user.id;
    this.reservData.propertyId = this.property.id.toString();
    //this.reservData.commission = 0;
    //this.reservData.discount = 0;
    this.reservData.advancepayment = 0;
    this.reservData.partpayment = 0;
    this.reservData.noOfRooms = this.roomDetails.length.toString();
    this.reservData.noOfGuests = this.roomDetails.reduce((sum, row) => sum + (row['years06'] || 0), 0)
      + this.roomDetails.reduce((sum, row) => sum + (row['years612'] || 0), 0)
      + this.roomDetails.reduce((sum, row) => sum + (row['years12'] || 0), 0);
    this.reservData.rooms = this.roomDetails;
    let sum = 0;
    this.roomDetails.forEach(element => {
      sum += element.newbamt - element.discount;
    });
    this.reservData.finalAmount = sum;
    this.reservData.balancepayment = sum;

    //this.paymentchange();

    this.dialogRef = this.dialogService.open(
      this.reserveDialogNew,
      { context: { title: 'Create Reservation' } });

  }

  paymentchange() {
    let sum = 0;

    this.roomDetails.forEach(element => {
      sum += element.newbamt - element.discount;
    });


    //let commission = this.reservData.commission;
    this.reservData.finalAmount = sum;
    //this.reservData.finalAmount = sum + commission;
    let famt = this.reservData.finalAmount;
    //let discount = this.reservData.discount;
    let advance = this.reservData.advancepayment;
    let part = this.reservData.partpayment;

    if (famt.toString() == '' || !famt) {
      famt = 0;
    }
    //if (discount.toString() == '' || !discount) {
    //  discount = 0;
    //}
    if (advance.toString() == '' || !advance) {
      advance = 0;
    }
    if (part.toString() == '' || !part) {
      part = 0;
    }
    if (advance + part > famt) {
      advance = this.reservData.advancepayment = 0;
      part = this.reservData.partpayment = 0;
      this.reservData.balancepayment = famt;
      this.toast('Error', 'Amount doesn\'t matches!', 'danger');
      return;
    }
    else {
      this.reservData.balancepayment = famt - (advance + part);
    }
  }

  onNewReserveSubmit() {

    this.loadingSave = true;

    if (this.foundFile == true) {
      if (this.validate()) {
        if (this.reservData.bookingChannelId == "47" && (this.reservData.enquiry_ref == null || this.reservData.enquiry_ref == "")) {
          this.loadingSave = false;
          this.toast('Error', 'Please enter the Req Reference!', 'danger');
          return;
        }
        this.reservData.custPhone = this.reservData.custPhone?.toString();
        let headers = new HttpHeaders().set("Authorization", "Bearer " +
          this.token).set("Content-Type", "application/json");

        this.http.post('api/reservation/create-new'
          , this.reservData, { headers: headers }).subscribe((res: any) => {
            this.loadingSave = false;

            if (res.actionStatus === 'SUCCESS') {

              let model = res.data;
              if (model && model.id) {
                let res_id = model.id;

                let docheaders = new HttpHeaders().set("Authorization", "Bearer " +
                  this.token);

                this.http.put('api/reservation/create-add-doc?resid=' + res_id + '&doctype=' + this.defaultCreateDocTypeId,
                  this.createFormData, { headers: docheaders }).subscribe((docRes: any) => {
                    this.loadingDocSave = false;

                    if (docRes.actionStatus === 'SUCCESS') {
                      this.docfiles = '';
                      this.loadDocs();
                      this.toast('Success', 'Data saved successfully!', 'success');
                      this.createresdocs = [];
                      this.dialogRef.close();
                    } else {
                      this.toast('Error', 'Could not save document!', 'danger');
                    }
                  },
                    error => {
                      this.loadingDocSave = false;
                      this.toast('Error', 'Could not save document!', 'danger');
                      console.log('api/reservation/create-add-doc', error);
                      if (error.status === 401) {
                        this.router.navigate(['auth/login']);
                      }
                    });
              }


              this.loadInventories();


              //this.toast('Success', 'Data saved successfully!', 'success');
              this.roomDetails = [];
              this.reservData = new ReservData();
              this.reservData.id = 0;

            }
            else {

              this.loadingSave = false;
              this.toast('Error', 'Could not save data!', 'danger');


            }

          },
            error => {

              this.loadingSave = false;

              console.log('api/reservation/create', error);
              if (error.status === 401) {
                this.router.navigate(['auth/login']);
              }

            });


      }
    }
    else {
      this.loadingSave = false;
      this.toast('Error', 'Please upload the required documents!', 'danger');
    }

  }

  editroom(id) {
    this.roomData = this.roomDetails.find(_obj => _obj.id == id);

    this.getroominventory();

    this.dialogRef = this.dialogService.open(
      this.paxDetails,
      { context: { title: 'Update Room - ' + this.roomData.room } });
    this.loadingSave = false;
  }

  deleteroom(id, deleteRoomDialog: TemplateRef<any>, deleteRoomMsgDialog: TemplateRef<any>) {
    this.mode = 'UPDATE';
    this.roomData = this.roomDetails.find(_obj => _obj.id == id);

    this.getroominventory();
    this.deleteId = id;
    if (this.roomDetails.length === 1) {
      this.dialogRef = this.dialogService.open(
        deleteRoomMsgDialog,
        { context: { title: 'Delete Room' } });
    }
    else {
      this.dialogRef = this.dialogService.open(
        deleteRoomDialog,
        { context: { title: 'Delete Room' } });
    }
  }
  confirmDeleteRoom() {

    this.roomDetails = this.roomDetails.filter(_obj => _obj.id != this.roomData.id);
    this.paymentchange();
    this.dialogRef.close();

  }

  onReserveSubmit() {

    this.loadingSave = true;
    if (this.reservData.bookingChannelId == "47" && (this.reservData.enquiry_ref == null || this.reservData.enquiry_ref == "")) {
      this.loadingSave = false;
      this.toast('Error', 'Please enter the Req Reference!', 'danger');
      return;
    }

    this.validate();

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.reservData.custPhone = this.reservData.custPhone?.toString();
    this.reservData.rooms = this.roomDetails;
    this.http.put('api/reservation/update-new?id=' + this.reservData.id
      , this.reservData, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {


          this.loadInventories();

          this.toast('Success', 'Data saved successfully!', 'success');
          this.updatedialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;

          console.log('api/reservation/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onReserveStatusSubmit() {
    if (confirm("Are you sure to complete?")) {
      this.loadingStatusSave = true;

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      this.reservData.custPhone = this.reservData.custPhone?.toString();

      this.http.put('api/reservation/update-status?id=' + this.reservData.id
        , { res_status: 'COMPLETED' }, { headers: headers }).subscribe((res: any) => {
          this.loadingStatusSave = false;

          if (res.actionStatus === 'SUCCESS') {

            this.loadReservations();
            this.loadInventories();

            this.toast('Success', 'Data saved successfully!', 'success');
            this.dialogRef.close();
          }
          else {

            this.loadingStatusSave = false;
            this.toast('Error', res.message, 'danger');


          }

        },
          error => {

            this.loadingStatusSave = false;

            console.log('api/reservation/update-status', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }

          });


    }

  }

  onValidateSubmit() {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`,
      'Content-Type': 'application/json'
    });
    this.http.put(`api/reservation/validate-reservation?id=${this.reservData.id}`, {}, { headers: headers }).subscribe(
      (res: any) => {
        if (res.actionStatus === 'SUCCESS') {
          this.loadReservations();
          this.toast('Success', 'Validated successfully!', 'success');
          //this.dialogRef.close();
        } else {
          this.toast('Error', res.message, 'danger');
        }
      },
      error => {
        //this.loadingStatusSave = false;
        console.log('api/reservation/validate-reservation', error);
        if (error.status === 401) {
          // Handle unauthorized error
        }
      }
    );
  }

  validate() {
    let ct = this.channels.find(_ct => _ct.id == this.reservData.bookingChannelId);
    if (ct.channelName == 'voyehomes.com') {
      if (this.reservData.custEmail == '' || !this.reservData.custEmail
        || this.reservData.custPhone == '' || !this.reservData.custPhone) {
        this.loadingSave = false;
        this.toast('Error', 'Email or Phone No should not be empty!', 'danger');
        return false;
      }
      else {
        const emailexp: RegExp = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;
        const emailresult: boolean = emailexp.test(this.reservData.custEmail);

        if (!emailresult) {
          this.loadingSave = false;
          this.toast('Error', 'Enter a valid email id!', 'danger');
          return false;
        }

        const phoneexp: RegExp = /^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$/;
        const phoneresult: boolean = phoneexp.test(this.reservData.custPhone);

        if (!phoneresult) {
          this.loadingSave = false;
          this.toast('Error', 'Enter a valid 10 digit phone number!', 'danger');
          return false;
        }
      }
    }
    return true;
  }

  private loadDocTypes() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/doctype/get-list'
      , { headers: headers }).subscribe((res: any) => {


        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.doctypes = res.data;
            this.createdoctypes = res.data.filter(item => item.default_doc == 1);
            this.alldoctypes = res.data;
            // console.log(JSON.stringify(this.channels));
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/doctype/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  private loadDocs() {

    this.resdocs = [];
    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reservation/get-doc-list?resid=' + this.reservData.id
      , { headers: headers }).subscribe((res: any) => {


        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.resdocs = res.data;
            //console.log(JSON.stringify(this.resdocs));
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/reservation/get-doc-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  uploadDocs(files) {
    if ((files.length === 0) || (this.defaultDocTypeId === "0")) {
      return;
    }

    if (this.reservData.bookingChannelId == "47" && (this.reservData.enquiry_ref == null || this.reservData.enquiry_ref == "")) {
      this.loadingSave = false;
      this.toast('Error', 'Please enter the Req Reference!', 'danger');
      return;
    }

    const selectedDocType = this.doctypes.find(doc => doc.id == this.defaultDocTypeId);
    const doc_Abbr = selectedDocType.doc_abbr;
    let filename = this.reservData.id + doc_Abbr;
    if (this.defaultDocTypeId == "1" || this.defaultDocTypeId == "4" || this.defaultDocTypeId == "5" || this.defaultDocTypeId == "6") {
      if (this.reservData.enquiry_ref != null) {
        filename = filename + this.reservData.enquiry_ref.toUpperCase();
      }
    }

    this.loadingDocSave = true;

    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      let fileToUpload = <File>files[i];
      formData.append('file' + i, fileToUpload, filename);
    }


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token);

    this.http.put('api/reservation/add-doc?resid=' + this.reservData.id + '&doctype=' + this.defaultDocTypeId
      , formData, { headers: headers }).subscribe((res: any) => {
        this.loadingDocSave = false;

        if (res.actionStatus === 'SUCCESS') {
          this.docfiles = '';
          this.loadDocs();
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingDocSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/reservation/add-doc', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  createUploadDocs(files) {
    if ((files.length === 0) || (this.defaultCreateDocTypeId === "0")) {
      this.toast('Error', 'Invalid document!', 'danger');
      return;
    }

    if (this.reservData.bookingChannelId == "47" && (this.reservData.enquiry_ref == null || this.reservData.enquiry_ref == "")) {
      this.loadingSave = false;
      this.toast('Error', 'Please enter the Req Reference!', 'danger');
      return;
    }

    const selectedDocType = this.createdoctypes.find(doc => doc.id == this.defaultCreateDocTypeId);
    const doc_Abbr = selectedDocType.doc_abbr;
    let filename = doc_Abbr;

    //this.createdoctypes = this.createdoctypes.filter(doc => doc.id != this.defaultCreateDocTypeId);


    let displayfilename = filename;
    //if (filename.includes('HP')) {
    //  this.foundFile = true;
    //}
    if (this.defaultCreateDocTypeId == "1" || this.defaultCreateDocTypeId == "4" || this.defaultCreateDocTypeId == "5" || this.defaultCreateDocTypeId == "6") {
      if (this.reservData.enquiry_ref != null) {
        displayfilename = filename + this.reservData.enquiry_ref.toUpperCase();
        filename = this.defaultCreateDocTypeId + "|" + filename + "|" + this.reservData.enquiry_ref.toUpperCase();

      }
      else {
        filename = this.defaultCreateDocTypeId + "|" + filename;
      }
    }

    let filesArray: { key: string, file: File }[] = [];
    this.createFormData.forEach((value, key) => {
      if (value instanceof File) {
        filesArray.push({ key, file: value });
      }
    });
    let selectfilesArray = filesArray.find(item => item.file.name.startsWith(this.defaultCreateDocTypeId));
    if (selectfilesArray != null) {

      filesArray = filesArray.filter(item => !item.file.name.startsWith(this.defaultCreateDocTypeId));

      let updatedFormData = new FormData();
      filesArray.forEach(item => {
        updatedFormData.append(item.key, item.file);
      });

      this.createFormData = updatedFormData;

      this.createresdocs = this.createresdocs.filter(doc => !doc.formFilename.startsWith(this.defaultCreateDocTypeId));

      for (let i = 0; i < files.length; i++) {
        let fileToUpload = <File>files[i];

        this.createFormData.append('file' + i, fileToUpload, filename);
      }

      this.createresdocs.push({
        file_name: displayfilename,
        doc_type_name: selectedDocType.doc_type_name,
        formFilename: filename,
        createresdoc_id: this.defaultCreateDocTypeId
      });
    }
    else {
      for (let i = 0; i < files.length; i++) {
        let fileToUpload = <File>files[i];

        this.createFormData.append('file' + i, fileToUpload, filename);
      }


      this.createresdocs.push({
        file_name: displayfilename,
        doc_type_name: selectedDocType.doc_type_name,
        formFilename: filename,
        createresdoc_id: this.defaultCreateDocTypeId
      });

    }

    let allPresent = true;
    for (const docType of this.createdoctypes) {
      const found = this.createresdocs.some(resDoc => resDoc.createresdoc_id == docType.id);

      if (!found) {
        allPresent = false;
      }
    }

    //if (allPresent) {
      this.foundFile = allPresent;
    //}

    this.defaultCreateDocTypeId = '0';

  }

  deleteCreationFile(filename) {

    let filesArray: { key: string, file: File }[] = [];
    this.createFormData.forEach((value, key) => {
      if (value instanceof File) {
        filesArray.push({ key, file: value });
      }
    });

    filesArray = filesArray.filter(item => item.file.name != filename);

    let updatedFormData = new FormData();
    filesArray.forEach(item => {
      updatedFormData.append(item.key, item.file);
    });

    this.createFormData = updatedFormData;

    //let selecteddocs = this.createresdocs.find(doc => doc.formFilename == filename);
    //let selecteddocId = selecteddocs.createresdoc_id;
    //
    //let selectdoctype = this.alldoctypes.find(doc => doc.id == selecteddocId)
    //
    //this.createdoctypes.push(selectdoctype);

    this.createresdocs = this.createresdocs.filter(doc => doc.formFilename != filename);

    let allPresent = true;
    for (const docType of this.createdoctypes) {
      const found = this.createresdocs.some(resDoc => resDoc.createresdoc_id == docType.id);

      if (!found) {
        allPresent = false;
      }
    }
    //if (allPresent) {
    this.foundFile = allPresent;
    //}

  }


  deleteFile(id) {

    if (confirm("Are you sure to delete?")) {
      this.loadingDocSave = true;
      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token);
      this.http.delete('api/reservation/delete-doc?id=' + id
        , { headers: headers }).subscribe((res: any) => {
          this.loadingDocSave = false;

          if (res.actionStatus === 'SUCCESS') {
            this.docfiles = '';
            this.loadDocs();
            this.toast('Success', 'File deleted successfully!', 'success');
          }
          else {


            this.toast('Error', 'Could not save data!', 'danger');


          }

        },
          error => {

            this.loadingDocSave = false;
            this.toast('Error', 'Could not delete file!', 'danger');

            console.log('api/reservation/delete-doc', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }

          });
    }
  }

  openReserveDelete(deleteReserveDialog: TemplateRef<any>) {
    this.updatedialogRef.close();
    this.dialogRef = this.dialogService.open(
      deleteReserveDialog,
      { context: { title: 'Delete Reservation' } });
  }

  deleteReservation() {


    this.loadingDelete = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/reservation/delete-new?id=' + this.reservData.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Reservation deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadInventories();
          this.reservData = new ReservData();
          this.reservData.id = 0;
          this.roomDetails = [];
        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/reservation/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  channelchange() {
    let ct = this.channels.find(_ct => _ct.id == this.reservData.bookingChannelId);
    if (ct.channelName == 'voyehomes.com' || ct.channelName == 'Property Owner') {
      this.isDisabled = true;
      this.reservData.tds = 0;
    }
    else {
      this.isDisabled = false;

    }
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }

}
class Property {

  id: string;
  propertyTypeId: string;
  destinationId: string;
  title: string;
  thumbnail: string;
  address: string;
  city: string;
  propertyStatus: string;
  sortOrder: number;
  shortDescription: string;
  longDescription: string;
  latitude: number;
  longitude: number;
  state: string;
  country: string;
  displayRadius: number;
  maxOccupancy: number;
  roomCount: number;
  bathroomCount: number;
  metaTitle: string;
  metaKeywords: string;
  metaDescription: string;
  phone: string;
  email: string;
  reserveAlert: string;
  reserveAllowed: string;
}

class Room {

  id: number;
  roomTypeId: string;
  propertyId: number;
  title: string;
  description: string;
  typeName: string;
}

class ReservData {

  id: number;
  propertyId: string;
  custName: string;
  custEmail: string | undefined;
  custPhone: string | undefined;
  bookingChannelId: string;
  details: string;
  maxAvailableQty: number;
  noOfRooms: string;
  noOfGuests: number;
  finalAmount: number | undefined;
  advancepayment: number | undefined;
  partpayment: number | undefined;
  balancepayment: number | undefined;
  //discount: number | undefined;
  commission: number | undefined;
  tds: number | undefined;
  country: string;
  created_on: string;
  updated_on: string;
  created_by: string;
  updated_by: string;
  enquiry_ref: string;
  res_status: string;
  rooms: RoomData[];
  user_permission: string;
  booking_agent: string;
  validated_by: string;
  validated_date: string;
}

class RoomData {
  id: number;
  fromDate: Date;
  toDate: Date;
  roomId: string;
  room: string;
  years06: number | 0;
  years612: number | 0;
  years12: number | 0;
  noOfGuests: number;
  amount: number | undefined;
  discount: number | 0;
  newbamt: number | 0;
  comments: string;
}
class BookingChannel {
  id: string;
  channelName: string;
}
class InvData {

  roomId: string;
  propertyId: string;
  invDate: string;
  normalocc: number;
  maxadults: number;
  maxchildren: number;
  occupancy: string;
  occcount: number;
  rate: number;
}

class createresdocsData {
  file_name: string;
  doc_type_name: string;
  formFilename: string;
  createresdoc_id: string;
}
