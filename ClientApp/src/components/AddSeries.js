import React, { Component, useState} from 'react';
import authService from './api-authorization/AuthorizeService'

export class AddSeries extends Component {
    constructor(props) {
        super(props);
        this.inputValue = "";
        this.onSubmit = this.onSubmit.bind(this);

        this.state = { searchresults: [], loading: true};
    }
    
    onChange = event => 
    {
        this.inputValue = event.target.value;
    }

    onSubmit(e)
    {
        e.preventDefault();
        this.GetSeries(this.inputValue);
    }

    async GetSeries(series)
    {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/SearchSeries/' + series, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        console.log(data);
        this.setState({searchresults: data, loading: false})
    }

    static searchResultsTable(searchresults)
    {
        return(
            <div clasName='container'>
                {searchresults.map(searchresult =>
                    <div class='card m-1 bg-dark text-light'>
                        <div class="row">
                            <div class='col-md-2'> 
                                <img className='img-fluid mh-50' src={searchresult.bannerUrl} />
                            </div>
                            <div className='col-md'>
                                <div className='card-body'>
                                    <h4 className='card-title'>{searchresult.title}</h4>
                                    <p className='card-text'>{searchresult.description}</p>
                                    <button className='btn btn-primary'>Watch</button>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        )
    }


    render() {

        let searchResultsView = this.state.loading
        ? <></>
        : AddSeries.searchResultsTable(this.state.searchresults);
  

        return (
            <>
                <div className='input-group'>
                    <div className='form-outline'>
                        <input type='search' id='search' className='form-control' placeholder='Search' onChange={this.onChange} />
                    </div>
                    <button type='button' className='btn btn-primary' onClick={this.onSubmit}>
                        Search
                    </button>
                </div>
                {searchResultsView}
            </>
        );
    }
}