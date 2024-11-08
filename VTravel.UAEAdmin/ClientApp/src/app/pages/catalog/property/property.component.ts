import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbToastrService, NbComponentStatus  } from '@nebular/theme';
import '../ckeditor.loader';
import 'ckeditor';
import { UniqueSelectionDispatcher } from '@angular/cdk/collections';

@Component({
  selector: 'property',
  templateUrl: './property.component.html',
  styleUrls: ['./property.component.scss'],
})


export class PropertyComponent implements OnInit  {

  aboutDialogRef: any;
  attributeDialogRef: any;
  priceDialogRef: any;
  deleteDialogRef: any;
  roomDialogRef: any;
  imageDialogRef: any;
  profitdialogRef: any;
  chargeDialogRef: any;
  propertyImage=new PropertyImage();
  aboutEditMode = false;
  thumbnailEditMode = false;
  imageEditMode = false;
  isLoading = false;
  loadingPropertySave = false;
  loadingImageAlt = false;
  loadingImageSave = false;
  loadingAboutSave = false;
  loadingStatusSave = false;
  loadingImageDelete = false;
  loadingChargeSave = false;
  loadingDelete = false;
  loadingAttributeSave = false;
  loadingPriceSave = false;
  loadingRoomSave = false;
  loadingProfitSave = false;
  loadingProfitDelete = false;
  loadingChargeDelete = false;
  isAlert = false;
  alertMessage = '';
  alertStatus = '';
  property = new Property();
  room = new Room();
  appcharge = new AppCharges();
  altcontact = new AltContact();
  chargelist: any[];
  contactlist: any[];
  propertyAttribute = new PropertyAttribute();
  propertyPrice = new PropertyPrice();
  prices: any[];
  propertiesAll: any[];
  countries: any[];
  states: any[];
  cities: any[];
  propertyTypes: any[];
  destinations: any[];
  amenities: any[];
  images: any[];
  tags: any[];
  attributes: any[];
  attributesAll: any[];
  token: any;
  propertyAbout: string ;
  position = new Position();
  center = new Position();

  roomTypes: any[];
  rooms: any[];
  occupancylist: any[];
  occupancylist1: Occupancy[];
  meallist: Mealplan[];

  profitDetails: any[] = [];
  profitsharing = new ProfitSharing();
  channels: any[];
  categorylist: any[];
  subcategorylist: any[];
  category: number;
  subcategory: number;
  imagedetails = new Imagedetails();

  owners: any[];


  nearestAttrac = new NearestAttrac();
  loadingSave = false;
  attractions: any[];
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

    });
  }

  ngOnInit() {

    this.loadCountries();
    this.loadPropertyTypes();
    this.loadRoomTypes();
    this.loadDestinations();
    this.loadProperty();
    this.loadAttributesAll();
    this.loadOwners();

    
    this.loadcategory();
    this.loadsubcategory();
    // this.occupancylist = [
    //  {
    //    id: '1',
    //    title: 'Single',
    //    checked: false,
    //  },
    //  {
    //    id: '2',
    //    title: 'Double',
    //    checked: false,
    //  },
    //  {
    //    id: '3',
    //    title: 'Triple',
    //    checked: false,
    //  },
    //  {
    //    id: '4',
    //    title: 'Quad',
    //    checked: false,
    //  },
    //  {
    //    id: '5',
    //    title: 'Five Person',
    //    checked: false,
    //  },
    //  {
    //    id: '6',
    //    title: 'Six Person',
    //    checked: false,
    //  },
    // ]
  }
  loadPropertyTypes() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/propertytype/get-list'
      , { headers: headers }).subscribe((res: any) => {


        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.propertyTypes = res.data;

          }
        }

      },
        error => {
          console.log('api/propertytype/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadRoomTypes() {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/roomtype/get-list'
      , { headers: headers }).subscribe((res: any) => {



        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.roomTypes = res.data;

          }
        }

      },
        error => {

          console.log('api/roomtype/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadDestinations() {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/destination/get-list'
      , { headers: headers }).subscribe((res: any) => {



        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.destinations = res.data;

          }
        }

      },
        error => {

          console.log('api/destination/get-list', error)
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

  loadStates() {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/location/get-state-list?countryCode=' + this.property.country
      , { headers: headers }).subscribe((res: any) => {



        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.states = res.data;

          }
        }

      },
        error => {

          console.log('api/location/get-state-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadCities() {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/location/get-city-list?countryCode='
      + this.property.country + '&stateCode=' + this.property.state
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.cities = res.data;

          }
        }

      },
        error => {

          console.log('api/location/get-city-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadProperty() {

    let id = this.r.snapshot.paramMap.get('id');

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + id
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {


          this.property = res.data;
          this.setMap();
          this.loadStates();
          this.loadCities();
          this.loadAmenities();
          this.loadTags();
          this.loadAttributes();
          this.loadPrices();
          this.loadImages();
          this.loadRooms();
          this.loadBookingChannels();
          this.profitsharing.mode = 'Percentage';
          this.profitsharing.propertyId = this.property.id;
          this.loadProfit();
          this.profitsharing.include_food = '0';
          this.profitsharing.include_extra = '0';
          this.profitsharing.taxless_amount = '0';
          this.loadCharges();
          this.loadContacts();
          this.loadAttractions();
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

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-room-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.rooms = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-room-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadAmenities() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-amenity-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.amenities = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-amenity-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadImages() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-image-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.images = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-image-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  uniqueFilter(value, index, self) {
    return self.indexOf(value) === index;
  }

  //categorize(images: any[], c: string): any[] {
  //  this.categorized = images.filter(p => p.category == c);
  //  return this.categorized;
  //}

  transform(collection: any[], property: string): any {
    if (!collection) {
      return null;
    }

    const groupedCollection = collection.reduce((acc, obj) => {
      const key = obj[property];
      if (!acc[key]) {
        acc[key] = [];
      }
      acc[key].push(obj);
      return acc;
    }, {});

    return Object.keys(groupedCollection).map(key => ({
      key,
      items: groupedCollection[key],
    }));
  }

  loadTags() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-tag-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.tags = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-tag-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadAttributes() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-attribute-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          this.attributes = [];
          if (res.data.length > 0) {
            this.attributes = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-attribute-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadPrices() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-price-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          this.prices = [];
          if (res.data.length > 0) {
            this.prices = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-price-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadAttributesAll() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/attribute/get-list'
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          this.attributesAll = [];
          if (res.data.length > 0) {
            this.attributesAll = res.data;

          }
        }

      },
        error => {

          console.log('api/attribute/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadCharges() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-charge-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.chargelist = res.data;
          }
        }

      },
        error => {

          console.log('api/property/get-tag-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadContacts() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/contact/get-active-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.contactlist = res.data;
          }
        }

      },
        error => {

          console.log('api/property/get-tag-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  private loadBookingChannels() {

    this.isLoading = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/bookingchannel/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.channels = res.data;
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

  loadOwners() {

    this.isLoading = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/owner/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.owners = res.data;

          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/owner/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  togglehideprop(checked: boolean) {
    if (checked) {
      this.property.hideProperty = '1';
    }
    else {
      this.property.hideProperty = '0';
    }
  }

  togglegst(checked: boolean) {
    if (checked) {
      this.property.gst = '1';
    }
    else {
      this.property.gst = '0';
    }
  }

  toggleAmenity(checked: boolean, amenityId) {

    let propertyAmenity = new PropertyAmenity();
    propertyAmenity.amenityId = amenityId;
    if (checked) {
      propertyAmenity.status = 1;
    }
    else {
      propertyAmenity.status = 0;
    }
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-property-amenity?id=' + this.property.id
      , propertyAmenity, { headers: headers }).subscribe((res: any) => {
        this.loadingStatusSave = false;

        if (res.actionStatus === 'SUCCESS') {
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-property-amenity', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  toggleTag(checked: boolean, tagId) {

    let propertyTag = new PropertyTag();
    propertyTag.tagId = tagId;
    if (checked) {
      propertyTag.status = 1;
    }
    else {
      propertyTag.status = 0;
    }
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-property-tag?id=' + this.property.id
      , propertyTag, { headers: headers }).subscribe((res: any) => {
        this.loadingStatusSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-property-tag', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  toggleThumbnailEditMode() {
    this.thumbnailEditMode = !this.thumbnailEditMode;
  }
  toggleImageEditMode(mediaDialogNew: TemplateRef<any>) {
    this.imageEditMode = !this.imageEditMode;

    this.propertyImage = new PropertyImage();
    this.propertyImage.category = "0";
    this.propertyImage.subcategory = "0";
    

    this.imageDialogRef = this.dialogService.open(
      mediaDialogNew,
      { context: { title: 'Add Image' } });
  }

  loadcategory() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-image-category-list'
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.categorylist = res.data;

          }
        }

      },
        error => {

          console.log('api/property/get-image-category-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadsubcategory() {
    this.subcategorylist = [];
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-image-subcategory-list?category=' + this.propertyImage.categoryid
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.subcategorylist = res.data;
          }
        }

      },
        error => {

          console.log('api/property/get-image-subcategory-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  uploadThumbnail(files) {
    if (files.length === 0) {
      return;
    }

    const file = files.files[0];
    const reader = new FileReader();

    reader.onload = (e: any) => {
      const img = new Image();
      img.src = e.target.result;

      img.onload = () => {
        const width = img.width;
        const height = img.height;

        if (width < 540 || height < 600) {
          this.toast('Error', 'Image dimension should not be less than 540*600!', 'danger');
          return;
        } 
      };

      img.onerror = () => {
        this.toast('Error', 'Invalid image!', 'danger');
        return;
      };
    };

    this.loadingPropertySave = true ;

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token);

    this.http.put('api/property/update-thumbnail?id=' + this.property.id
      , formData, { headers: headers }).subscribe((res: any) => {
        this.loadingPropertySave = false;
        if (res.actionStatus === 'SUCCESS') {
          this.thumbnailEditMode = false;
          this.loadProperty();
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-thumbnail', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  uploadImages(files) {
    if (files.length === 0) {
      return;
    }

    this.loadingImageSave = true ;

    const formData = new FormData();
    for (let i = 0; i < files.length; i++){
      let fileToUpload = <File>files[i];
      formData.append('file'+i, fileToUpload, fileToUpload.name);
    }

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token);

    this.http.put('api/property/add-image?id=' + this.property.id + '&&category=' + this.propertyImage.categoryid + '&&subcategory=' + this.propertyImage.subcategoryid + '&&room=' + this.propertyImage.roomid
      , formData, { headers: headers }).subscribe((res: any) => {
        this.loadingImageSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.imageEditMode = false;
          this.loadImages();
          this.imageDialogRef.close();
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingImageSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/add-image', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  openAboutDialog(aboutDialog: TemplateRef<any>) {

    this.propertyAbout = this.property.longDescription;

    this.aboutDialogRef = this.dialogService.open(
      aboutDialog,
      { context: { title: 'Edit About' } });
  }
  closeAboutEditMode() {
    this.propertyAbout = '';
    this.aboutEditMode = false;
  }
  //
  updatePropertyAbout() {

    this.loadingAboutSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-about?id=' + this.property.id
      , {"longDescription": this.propertyAbout }, { headers: headers }).subscribe((res: any) => {
        this.loadingAboutSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.property.longDescription = this.propertyAbout;
          this.propertyAbout = '';
          this.aboutDialogRef.close();
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-about', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  updatePropertyStatus(propertyStatus) {


    this.loadingStatusSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-status?id=' + this.property.id
      , { "propertyStatus": propertyStatus }, { headers: headers }).subscribe((res: any) => {
        this.loadingStatusSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadProperty();
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-status', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  openDelete(deleteDialog: TemplateRef<any>) {
    this.deleteDialogRef=this.dialogService.open(
      deleteDialog,
      { context: { title: 'Delete Property' } });
  }
  deleteProperty() {


    this.loadingDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/property/delete?id=' + this.property.id
          , { headers: headers }).subscribe((res: any) => {
            this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Property deleted successfully!', 'success');
          this.deleteDialogRef.close();
          this.router.navigate(['/pages/catalog/properties']);
        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/property/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  updateLocation() {

    // console.log(JSON.stringify(this.position));
    this.loadingPropertySave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-location?id=' + this.property.id
      , { "latitude": this.position.lat, "longitude": this.position.lng },
      { headers: headers }).subscribe((res: any) => {
        this.loadingPropertySave = false;
        // console.log(JSON.stringify(res));
        if (res.actionStatus === 'SUCCESS') {
          this.center = this.position;
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-location', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  openAttributeDelete(deleteAttributeDialog: TemplateRef<any>, id) {

    this.propertyAttribute = new PropertyAttribute();
    this.propertyAttribute.id = id;

    this.deleteDialogRef = this.dialogService.open(
      deleteAttributeDialog,
      { context: { title: 'Delete Attribute' } });
  }

  openPriceDelete(deletePriceDialog: TemplateRef<any>, id) {

    this.propertyPrice = new PropertyPrice();
    this.propertyPrice.id = id;

    this.deleteDialogRef = this.dialogService.open(
      deletePriceDialog,
      { context: { title: 'Delete Price' } });
  }

  deleteAttribute() {


    this.loadingDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/property/delete-property-attribute?id=' + this.propertyAttribute.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Attribute deleted successfully!', 'success');
          this.deleteDialogRef.close();
          this.loadAttributes();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/property/delete-property-attribute', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  deletePrice() {


    this.loadingDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/property/delete-property-price?id=' + this.propertyPrice.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Price deleted successfully!', 'success');
          this.deleteDialogRef.close();
          this.loadPrices();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/property/delete-property-price', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  openAttribute(attributeDialog: TemplateRef<any>, id) {

    let attr = this.attributes.find(_attr => _attr.id == id);

    this.propertyAttribute = new PropertyAttribute();

    this.propertyAttribute.id = id;
    this.propertyAttribute.attributeId = attr.attributeId;
    this.propertyAttribute.longDescription = attr.longDescription;

    this.attributeDialogRef =  this.dialogService.open(
      attributeDialog,
      { context: { title: 'Update Attribute' } });


  }

  openPrice(priceDialog: TemplateRef<any>, id) {

    let prc = this.prices.find(_price => _price.id == id);

    this.propertyPrice = new PropertyPrice();

    this.propertyPrice.id = id;
    this.propertyPrice.mrp = prc.mrp;
    this.propertyPrice.price = prc.price;
    this.propertyPrice.priceName = prc.priceName;

    this.priceDialogRef = this.dialogService.open(
      priceDialog,
      { context: { title: 'Update Price' } });


  }

  openImage(imageDialog: TemplateRef<any>, id) {

    //this.propertyImage = this.images.find(_img => _img.id == id);

    let obj = this.images.find(_img => _img.id == id);

    this.propertyImage = new PropertyImage();
    this.propertyImage.id = id;
    this.propertyImage.url = obj.url;
    this.propertyImage.image_alt = obj.image_alt;
    this.propertyImage.categoryid = obj.categoryid;
    this.propertyImage.category = obj.category;
    this.loadsubcategory();
    this.propertyImage.subcategoryid = obj.subcategoryid;
    this.propertyImage.subcategory = obj.subcategory;
    this.propertyImage.roomid = obj.roomid;

    this.imageDialogRef = this.dialogService.open(
      imageDialog,
      { context: { title: 'Image' } });

    
  }


  deleteImage(id) {

    if (confirm("Are you sure to delete?")) {
      this.loadingImageDelete = true ;

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      this.http.delete('api/property/delete-property-image?id=' + id
        , { headers: headers }).subscribe((res: any) => {
          this.loadingImageDelete = false;

          if (res.actionStatus === 'SUCCESS') {
            this.loadImages();
            this.toast('Success', 'Image deleted successfully!', 'success');
            this.imageDialogRef.close();


          }
          else {


            this.toast('Error', 'Could not delete!', 'danger');


          }

        },
          error => {

            this.loadingImageDelete = false;
            this.toast('Error', 'Could not delete!', 'danger');

            console.log('api/property/delete-property-image', error);
            if (error.status === 401) {
              this.router.navigate(['/auth/login']);
            }

          });
    }



  }

  updateImageAlt() {

    this.loadingImageAlt = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-image?id=' + this.propertyImage.id
      , this.propertyImage, { headers: headers }).subscribe((res: any) => {
        this.loadingImageAlt = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingImageAlt = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.loadImages();
          this.imageDialogRef.close();
        }
        else {

          this.loadingImageAlt = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingImageAlt = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-image', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  openAttributeNew(attributeDialogNew: TemplateRef<any>) {

    this.propertyAttribute = new PropertyAttribute();
    this.attributeDialogRef= this.dialogService.open(
      attributeDialogNew,
      { context: { title: 'Add Attribute' } });


  }

  openPriceNew(priceDialogNew: TemplateRef<any>) {

    this.propertyPrice = new PropertyPrice();
    this.priceDialogRef = this.dialogService.open(
      priceDialogNew,
      { context: { title: 'Add Price' } });


  }

  onBasicFormSubmit() {


    this.loadingPropertySave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update?id=' + this.property.id
      , this.property, { headers: headers }).subscribe((res: any) => {
        this.loadingPropertySave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingPropertySave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onContactFormSubmit() {




    this.loadingPropertySave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-contact?id=' + this.property.id
      , this.property, { headers: headers }).subscribe((res: any) => {
        this.loadingPropertySave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingPropertySave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-contact', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onMetaFormSubmit() {


    this.loadingPropertySave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-meta?id=' + this.property.id
      , this.property, { headers: headers }).subscribe((res: any) => {
        this.loadingPropertySave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingPropertySave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPropertySave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-meta', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onNewAttributeSubmit() {

    this.loadingAttributeSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.propertyAttribute.propertyId = this.property.id;
    this.http.post('api/property/create-property-attribute'
      , this.propertyAttribute, { headers: headers }).subscribe((res: any) => {
        this.loadingAttributeSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadAttributes();
          this.loadingAttributeSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.attributeDialogRef.close();
        }
        else {

          this.loadingAttributeSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAttributeSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/create-property-attribute', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onNewPriceSubmit() {

    this.loadingPriceSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.propertyPrice.propertyId = this.property.id;
    this.http.post('api/property/create-property-price'
      , this.propertyPrice, { headers: headers }).subscribe((res: any) => {
        this.loadingPriceSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadPrices();
          this.loadingPriceSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.priceDialogRef.close();
        }
        else {

          this.loadingPriceSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPriceSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/create-property-price', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onAttributeSubmit() {

    this.loadingAttributeSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-property-attribute?id=' + this.propertyAttribute.id
      , this.propertyAttribute, { headers: headers }).subscribe((res: any) => {
        this.loadingAttributeSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadAttributes();
          this.loadingAttributeSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.attributeDialogRef.close();
        }
        else {

          this.loadingAttributeSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAttributeSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-property-attribute', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onPriceSubmit() {

    this.loadingPriceSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-property-price?id=' + this.propertyPrice.id
      , this.propertyPrice, { headers: headers }).subscribe((res: any) => {
        this.loadingPriceSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadPrices();
          this.loadingPriceSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.priceDialogRef.close();
        }
        else {

          this.loadingPriceSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingPriceSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-property-price', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }


  mapClick(event: google.maps.MouseEvent) {
    // console.log(JSON.stringify(event));
    // this.position = { lat: event.latLng.lat(), lng: event.latLng.lng() };

    this.position = new Position();
    this.position.lat = event.latLng.lat();
    this.position.lng = event.latLng.lng();

  }

  setMap() {
    this.position = new Position();
    this.position.lat = this.property.latitude;
    this.position.lng = this.property.longitude;
    this.center = this.position;
  }

  openRoomNew(roomDialogNew: TemplateRef<any>) {

    this.room = new Room();
    this.room.id = 0;
    this.room.baserate = 0;
    this.room.noofrooms = 0;
    this.room.normalocc = 0;
    this.room.maxchildren = 0;
    this.room.maxadults = 0;
    this.loadRoomOccupancy();
    this.loadRoomMeals();
    this.roomDialogRef = this.dialogService.open(
      roomDialogNew,
      { context: { title: 'Add Room' } });


  }
  openRoom(roomDialog: TemplateRef<any>, id) {

    let obj = this.rooms.find(_obj => _obj.id == id);

    this.room = new Room();

    this.room.id = id;
    this.room.title = obj.title;
    this.room.description = obj.description;
    this.room.roomTypeId = obj.roomTypeId;
    this.room.noofrooms = obj.noofrooms;
    this.room.normalocc = obj.normalocc;
    this.room.maxadults = obj.maxadults;
    this.room.maxchildren = obj.maxchildren;
    this.room.baserate = obj.baserate;
    // this.room.years06 = obj.years06;
    // this.room.years612 = obj.years612;
    // this.room.years12 = obj.years12;

    this.loadRoomOccupancy();
    this.loadRoomMeals();

    this.roomDialogRef = this.dialogService.open(
      roomDialog,
      { context: { title: 'Update Room' } });


  }

  loadRoomOccupancy() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-room-occupancy?id=' + this.room.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.occupancylist1 = res.data;
          }
        }

      },
        error => {

          console.log('api/property/get-room-occupancy', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadRoomMeals() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-room-meals?id=' + this.room.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.meallist = res.data;
          }
        }

      },
        error => {

          console.log('api/property/get-room-occupancy', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }


  openRoomDelete(deleteRoomDialog: TemplateRef<any>, id) {

    this.room = new Room();
    this.room.id = id;

    this.deleteDialogRef = this.dialogService.open(
      deleteRoomDialog,
      { context: { title: 'Delete Room' } });
  }
  onNewRoomSubmit() {

    this.loadingRoomSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.room.propertyId = this.property.id;
    this.http.post('api/property/create-room'
      , this.room, { headers: headers }).subscribe((res: any) => {
        this.loadingRoomSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.http.put('api/property/update-room-occupancy?id=' + res.message
            , this.occupancylist1, { headers: headers }).subscribe((res: any) => {

            });
          this.http.put('api/property/update-room-mealplan?id=' + res.message
            , this.meallist, { headers: headers }).subscribe((res: any) => {

            });

          this.loadRooms();
          this.loadingRoomSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.roomDialogRef.close();
        }
        else {

          this.loadingRoomSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingRoomSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/create-room', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }
  onRoomSubmit() {

    this.loadingRoomSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-room?id=' + this.room.id
      , this.room, { headers: headers }).subscribe((res: any) => {
        // this.loadingRoomSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.http.put('api/property/update-room-occupancy?id=' + this.room.id
            , this.occupancylist1, { headers: headers }).subscribe((res: any) => {

            });
          this.http.put('api/property/update-room-mealplan?id=' + this.room.id
            , this.meallist, { headers: headers }).subscribe((res: any) => {

            });

          this.loadRooms();
          this.loadingRoomSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.roomDialogRef.close();
        }
        else {

          this.loadingRoomSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingRoomSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-room', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  toggleoccupancy(checked: boolean, id) {
    let attr = this.occupancylist1.find(_attr => _attr.id == id);

    if (checked) {
      attr.check = 'true';
    }
    else {
      attr.check = 'false';
    }
    this.maxchange();
  }

  togglemeals(checked: boolean, id) {
    let attr = this.meallist.find(_attr => _attr.id == id);

    if (checked) {
      attr.check = 'true';
    }
    else {
      attr.check = 'false';
    }
  }
  deleteRoom() {


    this.loadingDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/property/delete-room?id=' + this.room.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Room deleted successfully!', 'success');
          this.deleteDialogRef.close();
          this.loadRooms();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/property/delete-room', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  maxchange() {
    var maxcount = 0;

    for (var i = 0; i < this.occupancylist1.length; i++) {
      if (this.occupancylist1[i].check == 'true') {
        if (maxcount < this.occupancylist1[i].occcount) {
          maxcount = this.occupancylist1[i].occcount;
        }
      }
    }

    if (this.room.maxadults + this.room.maxchildren > maxcount) {
      this.room.maxadults = 0;
      this.room.maxchildren = 0;
      this.toast('Error', 'Max. count should not be greater than the occupancy!', 'danger');
      return false;
    }
  }

  toggleprofit(evt, mode) {
    if (evt) {
      if (mode == 'food') {
        this.profitsharing.include_food = '1';
      }
      else if (mode == 'extra') {
        this.profitsharing.include_extra = '1';
      }
      else {
        this.profitsharing.taxless_amount = '1';
      }
    }
    else {

      if (mode == 'food') {
        this.profitsharing.include_food = '0';
      }
      else if (mode == 'extra') {
        this.profitsharing.include_extra = '0';
      }
      else {
        this.profitsharing.taxless_amount = '0';
      }
    }
  }

  loadProfit() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/profit/get-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.profitDetails = res.data;
          }
        }

      },
        error => {

          console.log('api/profit/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  onProfitAdd() {

    this.loadingProfitSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    let attr = this.profitDetails.find(_attr => _attr.channelId == this.profitsharing.channelId);

    var url = '';
    if (attr!=null) {
      url = 'api/profit/update?id='+ attr.id;
    }
    else {
      url = 'api/profit/create';
    }

    this.http.post(url
      , this.profitsharing, { headers: headers }).subscribe((res: any) => {
        this.loadingProfitSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingProfitSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.profitsharing = new ProfitSharing();
          this.profitsharing.propertyId = this.property.id;
          this.profitsharing.mode = 'Percentage';
          this.loadProfit();
          this.profitsharing.include_food = '0';
          this.profitsharing.include_extra = '0';
          this.profitsharing.taxless_amount = '0';
        }
        else {

          this.loadingProfitSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
      error => {

        this.loadingProfitSave = false;
        this.toast('Error', 'Could not save data!', 'danger');

        console.log('api/profit/create', error);
        if (error.status === 401) {
          this.router.navigate(['auth/login']);
        }

      });
  }

  deleterow(id, deleteProfitDialog: TemplateRef<any>) {
    let attr = this.profitDetails.find(_attr => _attr.id == id);

    if (attr != null) {
      this.profitsharing = new ProfitSharing();
      this.profitsharing.id = attr.id;

      // this.profitdialogRef.close();
      this.profitdialogRef = this.dialogService.open(
        deleteProfitDialog,
        { context: { title: 'Delete Profit' } });
    }
    else {
      this.toast('Error', 'Could not load data!', 'danger');
    }
  }

  deleteProfit() {
    this.loadingProfitDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/profit/delete?id=' + this.profitsharing.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Profit Details deleted successfully!', 'success');
          this.profitdialogRef.close();
          this.profitsharing = new ProfitSharing();
          this.profitsharing.propertyId = this.property.id;
          const option3 = document.getElementById("rdFixed") as HTMLInputElement;
          option3.checked = true ;
          this.profitsharing.mode = 'Percentage';
          this.loadProfit();
          this.profitsharing.include_food = '0';
          this.profitsharing.include_extra = '0';
          this.profitsharing.taxless_amount = '0';

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/profit/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });
  }

  openChargeNew(chargeDialogNew: TemplateRef<any>) {
    this.appcharge = new AppCharges();
    this.appcharge.id = 0;
    this.appcharge.propertyid = this.property.id;
    this.appcharge.chargetype = 'Amount';
    this.chargeDialogRef = this.dialogService.open(
      chargeDialogNew,
      { context: { title: 'Add Charge' } });
  }

  openCharge(chargeDialog: TemplateRef<any>, id) {

    let obj = this.chargelist.find(_obj => _obj.id == id);

    this.appcharge = new AppCharges();

    this.appcharge.id = id;
    this.appcharge.propertyid = obj.propertyid;
    this.appcharge.name = obj.name;
    this.appcharge.chargetype = obj.chargetype;
    this.appcharge.amount = obj.amount;
    this.appcharge.percentage = obj.percentage;
    this.appcharge.effective = obj.effective;

    this.chargeDialogRef = this.dialogService.open(
      chargeDialog,
      { context: { title: 'Update Charge' } });


  }

  openChargeDelete(deleteChargeDialog: TemplateRef<any>, id) {

    this.appcharge = new AppCharges();
    this.appcharge.id = id;

    this.deleteDialogRef = this.dialogService.open(
      deleteChargeDialog,
      { context: { title: 'Delete Applicable Charge' } });
  }

  onNewChargeSubmit() {
    if (this.validateFields()) {
      this.loadingChargeSave = true ;

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      this.appcharge.propertyid = this.property.id;
      this.appcharge.effective = this.formatDate(this.appcharge.effective);

      this.http.post('api/property/create-charge'
        , this.appcharge, { headers: headers }).subscribe((res: any) => {
          this.loadingChargeSave = false;

          if (res.actionStatus === 'SUCCESS') {

            this.loadCharges();
            this.loadingChargeSave = false;
            this.toast('Success', 'Data saved successfully!', 'success');
            this.chargeDialogRef.close();
          }
          else {

            this.loadingChargeSave = false;
            this.toast('Error', 'Could not save data!', 'danger');


          }

        },
          error => {

            this.loadingChargeSave = false;
            this.toast('Error', 'Could not save data!', 'danger');

            console.log('api/property/create-charge', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }

          });
    }
  }

  onChargeSubmit() {
    if (this.validateFields()) {
      this.loadingChargeSave = true ;

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      this.appcharge.effective = this.formatDate(this.appcharge.effective);

      this.http.put('api/property/update-charge?'
        , this.appcharge, { headers: headers }).subscribe((res: any) => {

          if (res.actionStatus === 'SUCCESS') {

            this.loadCharges();
            this.loadingChargeSave = false;
            this.toast('Success', 'Data saved successfully!', 'success');
            this.chargeDialogRef.close();
          }
          else {

            this.loadingChargeSave = false;
            this.toast('Error', 'Could not save data!', 'danger');


          }

        },
          error => {

            this.loadingChargeSave = false;
            this.toast('Error', 'Could not save data!', 'danger');

            console.log('api/property/update-charge', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }

          });


    }

  }

  deletecharge() {
    this.loadingChargeDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/property/delete-charge?id=' + this.appcharge.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingChargeDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Applicable Charge deleted successfully!', 'success');
          this.deleteDialogRef.close();
          this.appcharge = new AppCharges();
          this.loadCharges();
        }
        else {

          this.loadingChargeDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingChargeDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/property/delete-charge', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });
  }

  validateFields(): boolean {

    // Check if the effective from date is in the future (optional)
    const currentDate = new Date();

    if (new Date(this.appcharge.effective).setHours(0, 0, 0, 0) < currentDate.setHours(0, 0, 0, 0)) {
      this.toast('Error', 'Effective From date should not be less than current date!', 'danger');
      this.appcharge.effective = '';
      return false;
    }


    if (this.appcharge.percentage > 100) {
      this.toast('Error', 'Percentage should not be greater than 100!', 'danger');
      this.appcharge.percentage = 0;
      return false;
    }

    // All checks passed, the dates are valid
    return true ;
  }

  togglecontact(checked: boolean, contactId) {
    let propertyContact = new AltContact();
    propertyContact.contact = contactId;
    if (checked) {
      propertyContact.status = 1;
    }
    else {
      propertyContact.status = 0;
    }
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/property/update-property-contact?id=' + this.property.id
      , propertyContact, { headers: headers }).subscribe((res: any) => {
        this.loadingStatusSave = false;

        if (res.actionStatus === 'SUCCESS') {
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {


          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/property/update-property-contact', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
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


  private loadAttractions() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/nearest-attraction/get-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        console.log(JSON.stringify(res));
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            console.log(res.data);
            this.attractions = res.data;
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/nearest-attraction/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  openAttractionNew(nearestAttracDialogNew: TemplateRef<any>) {
    this.nearestAttrac = new NearestAttrac();
    this.dialogRef = this.dialogService.open(
      nearestAttracDialogNew,
      { context: { title: 'Add Nearest Attraction' } });


  }

  openAttraction(attractionDialog: TemplateRef<any>, id) {

    let attr = this.attractions.find(_am => _am.id == id);

    this.nearestAttrac = new NearestAttrac();

    this.nearestAttrac.id = id;

    this.nearestAttrac.attractionName = attr.attractionName;
    this.nearestAttrac.image = attr.image;
    this.nearestAttrac.attractLocation = attr.attractLocation;
    this.nearestAttrac.distance = attr.distance;
    this.dialogRef = this.dialogService.open(
      attractionDialog,
      { context: { title: 'Update Nearest Attraction' } });


  }

  openAttractionDelete(deleteAttractionDialog: TemplateRef<any>, id) {

    this.nearestAttrac = new NearestAttrac();

    this.nearestAttrac.id = id;

    this.dialogRef = this.dialogService.open(
      deleteAttractionDialog,
      { context: { title: 'Delete Amenity' } });
  }

  onNewAttractionSubmit() {

    this.loadingSave = true;

    console.log(this.property.id);
    this.nearestAttrac.propertyid = this.property.id;
    console.log(JSON.stringify(this.nearestAttrac));
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.post('api/nearest-attraction/create'
      , this.nearestAttrac, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadAttractions();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/nearest-attraction/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onAttractionSubmit() {

    this.loadingSave = true;
    //console.log(this.nearestAttrac.attractLocation);
    this.nearestAttrac.propertyid = this.property.id;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/nearest-attraction/update?id=' + this.nearestAttrac.id
      , this.nearestAttrac, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadAttractions();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/nearest-attraction/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteAttraction() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/nearest-attraction/delete?id=' + this.nearestAttrac.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'nearest-attraction deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadAttributes();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/nearest-attraction/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }
}

class Property {

  id: number;
  propertyTypeId: string ;
  destinationId: string ;
  title: string ;
  thumbnail: string ;
  address: string ;
  city: string ;
  propertyStatus: string ;
  sortOrder: number;
  shortDescription: string ;
  longDescription: string ;
  latitude: number;
  longitude: number;
  state: string ;
  country: string ;
  displayRadius: number;
  maxOccupancy: number;
  roomCount: number;
  bathroomCount: number;
  metaTitle: string ;
  metaKeywords: string ;
  metaDescription: string ;
  phone: string ;
  email: string ;
  reserveAlert: string ;
  reserveAllowed: string ;
  hideProperty: string ;
  gst: string ;
  owner: string ;
  userName: string ;
}

class PropertyAmenity {

  status: number;
  amenityId: number;
  propertyId: number;
}

class PropertyTag {

  status: number;
  tagId: number;
  propertyId: number;
}

class PropertyAttribute {

  id: number;
  attributeId: string ;
  propertyId: number;
  attributeName: string ;
  longDescription: string ;
}

class PropertyPrice {

  id: number;
  propertyId: number;
  priceName: string ;
  mrp: number;
  price: number;
}

class Position {

  lat = 10.0889;
  lng = 77.0595;

}


class Room {

  id: number;
  roomTypeId: string ;
  propertyId: number;
  title: string ;
  description: string ;
  typeName: string ;
  noofrooms: number;
  normalocc: number;
  maxadults: number;
  maxchildren: number;
  baserate: number;
  // years06: number;
  // years612: number;
  // years12: number;
  // occupancy: Occupancy;s
}

class Occupancy {

  id: number;
  room_id: number;
  occupancy: string;
  check: string;
  occcount: number;

}

class Mealplan {

  id: number;
  room_id: number;
  mealplan: string;
  check: string;

}

class PropertyImage {

  id: number;
  url: number;
  image_alt: string ;
  categoryid: string;
  category: string;
  subcategoryid: string;
  subcategory: string;
  roomid: string;
}


class ProfitSharing {

  id: number;
  propertyId: number;
  room: string ;
  channelId: number;
  channel: string ;
  mode: string ;
  price: number;
  percentage: number ;
  include_food: string ;
  include_extra: string ;
  taxless_amount: string ;
}

class AppCharges {
  id: number;
  propertyid: number;
  name: string;
  chargetype: string;
  amount: number;
  percentage: number;
  effective: string;
}

class AltContact {
  id: number;
  propertyid: number;
  contact: number;
  status: number;
}
class Imagedetails {
  category: number;
  subcategory: number;
  imagealt: number;
}

class NearestAttrac {
  id: number;
  propertyid: number;
  attractionName: string;
  image: string;
  attractLocation: string;
  distance: number;
}

