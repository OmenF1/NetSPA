import React, { Component} from 'react';
import authService from './api-authorization/AuthorizeService';

export class AddSeries extends Component {
    constructor(props) {
        super(props);
        this.inputValue = "";
        this.onSubmit = this.onSubmit.bind(this);
        this.onWatch = this.onWatch.bind(this);
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

    async onWatch(series)
    {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/Watch/' + series, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        console.log(data);
    }

    async GetSeries(series)
    {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/SearchSeries/' + series, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        this.setState({searchresults: data, loading: false})
    }

    static searchResultsCards(searchresults, onWatch)
    {

        return(
            <div className='container'>
                {searchresults.map(searchresult =>
                    <div className='card m-1 bg-dark text-light' key={searchresult.id}>
                        <div className="row">
                            <div className='col-md-2'> 
                                <img className='img-fluid mh-50' alt={searchresult.title} src={searchresult.bannerUrl} />
                            </div>
                            <div className='col-md'>
                                <div className='card-body'>
                                    <h4 className='card-title'>{searchresult.title}</h4>
                                    <p className='card-text'>{searchresult.description}</p>
                                    <button type='button' className='btn btn-primary' onClick={() => onWatch(searchresult.id)}>
                                        Watch
                                    </button>
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
        : AddSeries.searchResultsCards(this.state.searchresults, this.onWatch);
  

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