import React, { Component} from 'react';
import authService from './api-authorization/AuthorizeService';


export class ListWatching extends Component {
    constructor(props) {
        super(props);
        this.state = { userShows: [], loading: true};
        this.deleteShow = this.deleteShow.bind(this);
        this.nextShow = this.nextShow.bind(this);
    }

    componentDidMount() {
        this.fetchUserShows();
    }

    async deleteShow(series) {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/RemoveSeries/' + series, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        this.fetchUserShows();
        console.log(response.json);
    }

    async nextShow(series) {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/NextEpisode/' + series, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        this.fetchUserShows();
        console.log(response.json);
    }

    async fetchUserShows() {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/Get', {
          headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        this.setState({ userShows: data, loading: false });
    }

    static searchResultsCards(usershows, deleteShow, nextShow)
    {
        return(
            <div className='container'>
                {usershows.map(show =>
                    <div className='card m-1 bg-dark text-light' key={show.id}>
                        <div className="row">
                            <div className='col-md-2'> 
                                <img className='img-fluid mh-50' alt={show.title} src={show.bannerUrl} />
                            </div>
                            <div className='col-md'>
                                <div className='card-body'>
                                    <h4 className='card-title'>{show.title}</h4>
                                    <p className='card-text'>{show.description}</p>
                                    <hr />
                                    <h5>Season {show.currentSeason} Episode {show.currentEpisode}</h5>
                                    <button type='button' className='btn btn-primary' onClick={() => nextShow(show.id)}>
                                        Next
                                    </button>
                                    <button type='button' className='btn btn-danger' onClick={() => deleteShow(show.id)}>
                                        Delete
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
        : ListWatching.searchResultsCards(this.state.userShows, this.deleteShow, this.nextShow);

        return (
            <div>
                {searchResultsView}
            </div>
        )
    }

    
}